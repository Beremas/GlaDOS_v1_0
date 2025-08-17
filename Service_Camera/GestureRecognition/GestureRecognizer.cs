using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using OpenCvSharp;
using System.IO;

namespace GlaDOS_v1_0.Service_Camera.GestureRecognition
{
    public class GestureRecognizer : IDisposable
	{
		private readonly InferenceSession _session;
		private readonly string[] _gestureLabels;

		public GestureRecognizer(string modelPath, string[] gestureLabels)
		{
			_session = new InferenceSession(modelPath);
			_gestureLabels = gestureLabels;
		}

		public string PredictGesture(Mat frame)
		{
			var inputTensor = PreprocessImage(frame);
			var inputs = new List<NamedOnnxValue>
				{
					NamedOnnxValue.CreateFromTensor("input", inputTensor)
				};

			using var results = _session.Run(inputs);
			var output = results.First().AsEnumerable<float>().ToArray();

			if (output.Length == 0)
				return "Unknown";

			int maxIndex = Array.IndexOf(output, output.Max());

			if (maxIndex >= _gestureLabels.Length)
				return "Unknown";

			return _gestureLabels[maxIndex];
		}

		private DenseTensor<float> PreprocessImage(Mat frame)
		{
			const int targetWidth = 192;
			const int targetHeight = 192;

			using var resized = frame.Resize(new Size(targetWidth, targetHeight));
			using var rgb = resized.CvtColor(ColorConversionCodes.BGR2RGB);

			var input = new DenseTensor<float>(new[] { 1, 3, targetHeight, targetWidth });

			for (int y = 0; y < targetHeight; y++)
			{
				for (int x = 0; x < targetWidth; x++)
				{
					var pixel = rgb.At<Vec3b>(y, x);
					input[0, 0, y, x] = pixel.Item0 / 255.0f; // R
					input[0, 1, y, x] = pixel.Item1 / 255.0f; // G
					input[0, 2, y, x] = pixel.Item2 / 255.0f; // B
				}
			}

			return input;
		}

		public void Dispose()
		{
			_session?.Dispose();
		}
	}
}
