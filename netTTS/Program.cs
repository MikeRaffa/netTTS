// -*- mode: java; -*-

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Speech.Synthesis;
using System.Diagnostics;

namespace netTTS {

internal class Program {
    static void Main (string[] args) {
        if (args.Length > 1) {
            Console.WriteLine("USAGE: netTTS [ <port> ]");
            Environment.Exit(101);
        }
        else if (args.Length == 1) {
            Program.port = int.Parse(args[0]);
        }
        StartServer();
    }

    private static void StartServer () {
        var ipAddress = GetLocalIPAddress();
        IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);
        try {
            Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(localEndPoint);
            listener.Listen(5);
            serveConnections(listener);
        }
        catch (Exception e) {
            Console.WriteLine(e.ToString());
// NB:  Eat it, maybe things will still work?
        }
    }

    private static void serveConnections (Socket listener) {
        writePidFile();
        SpeechSynthesizer synthesizer = new SpeechSynthesizer();
        while (true) {
            Console.WriteLine("netTTS: Waiting for a connection on: " + Dns.GetHostName() + ' ' + GetLocalIPAddress().ToString() + ' ' + port);
            Socket sock = listener.Accept();
            serveRequest(sock, synthesizer);
        }
    }

    private static void serveRequest (Socket sock, SpeechSynthesizer synthesizer) {
        int datLen = 0;
        int outNum = 0;
        int outBasis = 0;
        byte[] bytes = null;
        int chunkSz = 1024;
        string data = null;
        while (true) {
            bytes = new byte[chunkSz];
            int bytesRec = sock.Receive(bytes);
            //Console.WriteLine("DLD| Got " + bytesRec + " bytes");
            data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
            // NB: We are /not/ worried about edge cases
            // around network congestion here.  Experience whows (for me) that
            // I don't get problems with  chunks that are
            // not the last chunk, but have less than chunk size.
            // If I did I would make the chunk size smaller
            // before complicating this, I think.
            if (bytesRec < chunkSz) {
                datLen = data.Length;
                Console.WriteLine("netTTS: got data" + data);
                break;
            }
        }
        if (data.Equals("@->-GET-VOLUME-<-@\n")) {
            getVolumeRequest(sock, synthesizer);
        }
        else if (parseSetCommand(data, "@->-SET-VOLUME", out outNum, out outBasis)) {
            setVolumeRequest(sock,synthesizer, outNum, outBasis);
        }
        else if (data.Equals("@->-GET-RATE-<-@\n")) {
            getRateRequest(sock, synthesizer);
        }
        else if (parseSetCommand(data, "@->-SET-RATE", out outNum, out outBasis)) {
            setRateRequest(sock, synthesizer, outNum, outBasis);
        }
        else if (data.Substring(0, 4).Equals("@->-")) {
            // Command that didn't parse, ignore it.
            sock.Shutdown(SocketShutdown.Both);
            sock.Close();
        }
        else {
            // NB:  I would like to use a NetworkStream(sock), passing it to SetOutputToWaveStream,
            // but the synthesizer requires a stream that is seekable.  With a NetworkStream:
            // System.NotSupportedException: This stream does not support seek operations.
            // which is thrown at runtime.  Alas, just copy into a MemoryStream and then write
            // that into the socket.  Ick.
            using (MemoryStream mS = new MemoryStream()) {
                synthesizer.SetOutputToWaveStream(mS);
                try {
                    synthesizer.SpeakSsml(data);
                }
                catch (System.FormatException) {
                    Console.WriteLine("netTTS: Not XML, trying as plain text...");
                    synthesizer.Speak(data);
                }
                mS.Seek(0, SeekOrigin.Begin);
                byte[] oBS = new byte[mS.Length];
                int cnt = mS.Read(oBS, 0, (int)(mS.Length));
                sock.Send(oBS);
                sock.Shutdown(SocketShutdown.Both);
                sock.Close();
            }
        }
    }

    private static bool parseSetCommand (string data, string prefix, out int number, out int basis) {
        number = 0;
        basis = 0;
        int dLen = data.Length;
        int pLen = prefix.Length;
        if (dLen <= pLen) {
            return false;
        }
        if ( ! data.Substring(0, pLen).Equals(prefix)) {
            return false;
        }
        string suf = data.Substring(dLen - 5);
        if ( ! suf.Equals("-<-@\n")) {
            return false;
        }
        else {
            string arg = data.Substring(pLen, dLen - pLen - 5);
            int plusIdx = arg.IndexOf("+");
            int minusIdx = arg.IndexOf("-");
            int digIdx = 0;
            basis = 0;
            if (plusIdx >= 0 && minusIdx >= 0) {
                return false;
            }
            else if (plusIdx >= 0) {
                digIdx = plusIdx + 1;
                basis = 1;
            }
            else if (minusIdx >= 0) {
                digIdx = minusIdx + 1;
                basis = -1;
            }
            int N;
            if ( ! int.TryParse(arg.Substring(digIdx), out N)) {
                return false;
            }
            else {
                number = N;
            }
        }
        return true;
    }

    private static bool inBoundsP (int value, int min, int max) {
        bool rV = value >= min && value <= max;
        return rV;
    }

    private static void getVolumeRequest (Socket sock, SpeechSynthesizer synthesizer) {
        int sV = synthesizer.Volume;
        byte [] sVB = Encoding.ASCII.GetBytes (sV.ToString());
        sock.Send(sVB);
        sock.Shutdown(SocketShutdown.Both);
        sock.Close();
    }

    private static void setVolumeRequest (Socket sock, SpeechSynthesizer synthesizer, int number, int basis) {
        if (basis == 0) {
            // Absolute setting.
            if (inBoundsP(number, 0, 100)) {
                synthesizer.Volume = number;
            }
        }
        else {
            // Positive or negative offset from current setting.
            int nV = synthesizer.Volume + (basis * number);
            if (inBoundsP(nV, 0, 100)) {
                synthesizer.Volume = nV;
            }
        }
        sock.Shutdown(SocketShutdown.Both);
        sock.Close();
    }

    private static void getRateRequest (Socket sock, SpeechSynthesizer synthesizer) {
        int sR = synthesizer.Rate;
        byte [] sRB = Encoding.ASCII.GetBytes (sR.ToString());
        sock.Send(sRB);
        sock.Shutdown(SocketShutdown.Both);
        sock.Close();
    }

    private static void setRateRequest (Socket sock, SpeechSynthesizer synthesizer, int number, int basis) {
        // FIXME:  Not sure what the bounds for rate are, just fire and forget, for now.
        if (basis == 0) {
            // Absolute setting.
            synthesizer.Rate = number;
        }
        else {
            // Positive or negative offset from current setting.
            synthesizer.Rate = synthesizer.Rate + (basis * number);
        }
        sock.Shutdown(SocketShutdown.Both);
        sock.Close();
    }

    private static IPAddress GetLocalIPAddress () {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList) {
            if (ip.AddressFamily == AddressFamily.InterNetwork) {
                return ip;
            }
        }
        throw new Exception("No network adapters with an IPv4 address in the system!");
    }

    private static void writePidFile () {
        int pid = Process.GetCurrentProcess().Id;
        string path = @"netTTS.pid";
        using (        StreamWriter sw = File.CreateText(path)) {
            sw.Write(pid.ToString());
        }
    }

    private static int port;
    }
}

// Ends here.
