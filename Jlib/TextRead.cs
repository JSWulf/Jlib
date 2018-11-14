using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech.Synthesis;
using System.Speech.Recognition;
using System.Speech.AudioFormat;
using System.Globalization;

namespace Jlib
{
    class TextRead
    {
        public static void TextReadAloud(string text)
        {
            SpeechSynthesizer speech = new SpeechSynthesizer();
            //speech.GetInstalledVoices();
            //speech.SelectVoiceByHints(VoiceGender.Female);
            speech.SetOutputToDefaultAudioDevice();
            //speech.p
            speech.Speak(text);
            speech.Dispose();


            var a = new SpeechRecognizer();
            a.SpeechDetected += delegate { var ab = ""; };
            RecognizedPhrase v;
        }
    }
}
