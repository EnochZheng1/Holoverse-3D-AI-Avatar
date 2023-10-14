using System;
using System.Speech.Synthesis;
using System.Speech.Recognition;
using System.Threading;
using System.Threading.Tasks;
using NAudio.Wave;

namespace AudioLibrary
{
    public class Service
    {
        public void ConvertTextToSpeech(string text)
        {
            using (SpeechSynthesizer synthesizer = new SpeechSynthesizer())
            {
                synthesizer.Speak(text);
            }
        }
        public void AmplifyAudio(string inputPath, string outputPath, float amplificationFactor)
        {
            using (var reader = new AudioFileReader(inputPath))
            {
                reader.Volume = amplificationFactor; // e.g., 1.0f is original, 2.0f is double volume, etc.
                WaveFileWriter.CreateWaveFile(outputPath, reader);
            }
        }
        public string ConvertSpeechToText(string audioFilePath)
        {
            using (SpeechRecognitionEngine recognizer = new SpeechRecognitionEngine())
            {
                recognizer.SetInputToWaveFile(audioFilePath);
                recognizer.LoadGrammar(new DictationGrammar());

                try
                {
                    RecognitionResult result = recognizer.Recognize();
                    if (result != null)
                    {
                        return result.Text;
                    }
                    else
                    {
                        return "Failed to recognize speech.";
                    }
                }
                catch (Exception ex)
                {
                    return $"Error: {ex.Message}";
                }
            }
        }

        public void RecordAudio(string outputFilePath, int durationInSeconds)
        {
            using (var waveIn = new WaveInEvent())
            using (var writer = new WaveFileWriter(outputFilePath, waveIn.WaveFormat))
            {
                waveIn.DataAvailable += (sender, e) =>
                {
                    writer.Write(e.Buffer, 0, e.BytesRecorded);
                };

                waveIn.StartRecording();
                Thread.Sleep(durationInSeconds * 1000);
                waveIn.StopRecording();
            }
        }

        public static void Main()
        {
            var speechService = new Service();

            // Convert text to speech
            speechService.ConvertTextToSpeech("Hello, how are you?");
            speechService.ConvertTextToSpeech("I like your new car what is it? LMAO");

            // Record audio
            string audioFilePath = "temp.wav";
            Console.WriteLine("Recording... Speak now.");
            speechService.RecordAudio(audioFilePath, 10); // Record for 10 seconds

            // Amplify the recorded audio
            string amplifiedAudioFilePath = "amplified_temp.wav";
            speechService.AmplifyAudio(audioFilePath, amplifiedAudioFilePath, 5.0f); // Double the volume

            // Convert speech to text using the amplified audio
            string recognizedText = speechService.ConvertSpeechToText(amplifiedAudioFilePath);
            Console.WriteLine($"Recognized Text: {recognizedText}");
        }
    }
}
