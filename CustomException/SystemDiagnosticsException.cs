using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlaDOS_v1_0.CustomException
{
	public class SystemDiagnosticsException: Exception
	{
		public SystemDiagnosticsException(string message) : base(message) { }
	}
}
