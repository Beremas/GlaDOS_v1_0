using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GlaDOS_v1_0.Service_Camera.EmotionRecognition
{
	public class EmotionRecognizer : IDisposable
	{
		private readonly InferenceSession _session;
		private readonly CascadeClassifier _faceDetector;
		private readonly string[] _emotionLabels =
		{
			"neutral", "happiness", "surprise", "sadness",
			"anger", "disgust", "fear", "contempt"
		};

		public EmotionRecognizer(string modelPath, string faceCascadePath)
		{
			_session = new InferenceSession(modelPath);
			_faceDetector = new CascadeClassifier(faceCascadePath);
		}

		public string PredictEmotion(Mat frame)
		{
			// Detect face(s)
			var grayFrame = frame.CvtColor(ColorConversionCodes.BGR2GRAY);
			var faces = _faceDetector.DetectMultiScale(grayFrame, 1.1, 3);

			if (faces.Length == 0)
				return "No face detected";

			// Use first detected face
			var faceRect = faces[0];
			var face = new Mat(grayFrame, faceRect);

			// Resize to 64x64
			Cv2.Resize(face, face, new Size(64, 64));

			face.ConvertTo(face, MatType.CV_32FC1);

			// Create input tensor [1, 1, 64, 64]
			var inputTensor = new DenseTensor<float>(new[] { 1, 1, 64, 64 });
			for (int y = 0; y < 64; y++)
			{
				for (int x = 0; x < 64; x++)
				{
					inputTensor[0, 0, y, x] = face.At<float>(y, x);
				}
			}

			var inputs = new List<NamedOnnxValue>
			{
				NamedOnnxValue.CreateFromTensor("Input3", inputTensor)
			};

			using var results = _session.Run(inputs);
			var output = results.First().AsEnumerable<float>().ToArray();

			int maxIndex = Array.IndexOf(output, output.Max());
			return _emotionLabels[maxIndex];
		}

		public void Dispose()
		{
			_session?.Dispose();
			_faceDetector?.Dispose();
		}
	}
}
