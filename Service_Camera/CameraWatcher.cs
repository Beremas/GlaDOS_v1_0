using GlaDOS_v1_0.Service_Camera.EmotionRecognition;
using OpenCvSharp;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace GlaDOS_v1_0.Service_Camera
{
	public class CameraWatcher : IDisposable
	{
		private readonly EmotionRecognizer _emotionRecognizer;
		private readonly CascadeClassifier _faceDetector;

		public CameraWatcher()
		{
			string modelPath = Path.Combine(AppContext.BaseDirectory, "Service_Camera", "EmotionRecognition", "Model", "emotion-ferplus-8.onnx");
			string cascadePath = Path.Combine(AppContext.BaseDirectory, "Service_Camera", "EmotionRecognition", "Model", "haarcascade_frontalface_default.xml");

			_emotionRecognizer = new EmotionRecognizer(modelPath, cascadePath);
			_faceDetector = new CascadeClassifier(cascadePath);
		}

		public void Start(Action<string, Rect> onEmotionDetected)
		{
			using var capture = new VideoCapture(0);
			using var window = new Window("Eyes");
			using var frame = new Mat();

			while (true)
			{
				capture.Read(frame);
				if (frame.Empty()) continue;

				// Detect faces
				var faces = _faceDetector.DetectMultiScale(frame);

				foreach (var face in faces)
				{
					// Draw face rectangle
					Cv2.Rectangle(frame, face, Scalar.Blue, 2);

					// Predict emotion
					string emotion = _emotionRecognizer.PredictEmotion(frame);

					onEmotionDetected?.Invoke(emotion, face);
					
					// Draw emotion label
					Cv2.PutText(frame, emotion, new Point(face.X, face.Y - 10), HersheyFonts.HersheySimplex, 0.9, Scalar.Yellow, 2);
				}

				// Show frame
				window.ShowImage(frame);

				if (Cv2.WaitKey(30) == 27) break; // ESC to exit
			}
		}

		public void Dispose()
		{
			_emotionRecognizer?.Dispose();
			_faceDetector?.Dispose();
		}
	}
}
