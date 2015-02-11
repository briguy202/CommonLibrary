using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections.Specialized;
using System.Reflection;
using System.Globalization;

namespace CommonLibrary.Framework.Exceptions {
	public class CustomDatabaseExceptionFormatter : Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.ExceptionFormatter {
		private readonly TextWriter _writer;

		/// <summary>
		/// Initializes a new instance of the 
		/// <see cref="TextExceptionFormatter"/> using the specified
		/// <see cref="TextWriter"/> and <see cref="Exception"/>
		/// objects.
		/// </summary>
		/// <param name="writer">The stream to write formatting information to.</param>
		/// <param name="exception">The exception to format.</param>
		public CustomDatabaseExceptionFormatter(TextWriter writer, Exception exception) : base(exception, new Guid()) {
			if (writer == null) throw new ArgumentNullException("writer");

			_writer = writer;
		}

		/// <summary>
		/// Gets the underlying <see cref="TextWriter"/>
		/// that the current formatter is writing to.
		/// </summary>
		public TextWriter Writer {
			get { return _writer; }
		}

		/// <summary>
		/// Writes a generic description to the underlying text stream.
		/// </summary>
		protected override void WriteDescription() {
			this.Writer.Write(string.Format("Type: {0}", base.Exception.GetType().FullName));
		}

		/// <summary>
		/// Writes the current date and time to the <see cref="TextWriter"/>.
		/// </summary>
		/// <param name="utcNow">The current time.</param>
		protected override void WriteDateTime(DateTime utcNow) {
			DateTime localTime = utcNow.ToLocalTime();
			string localTimeString = localTime.ToString("G", DateTimeFormatInfo.InvariantInfo);

			this.Writer.Write(localTimeString);
		}

		/// <summary>
		/// Writes the value of the <see cref="Type.AssemblyQualifiedName"/>
		/// property for the specified exception type to the <see cref="TextWriter"/>.
		/// </summary>
		/// <param name="exceptionType">The <see cref="Type"/> of the exception.</param>
		protected override void WriteExceptionType(Type exceptionType) {
			this.Writer.Write(string.Format("Type: {0}", exceptionType.AssemblyQualifiedName));
		}

		/// <summary>
		/// Writes the value of the <see cref="Exception.Message"/>
		/// property to the underyling <see cref="TextWriter"/>.
		/// </summary>
		/// <param name="message">The message to write.</param>
		protected override void WriteMessage(string message) {
			this.Writer.Write(string.Format("Exception: {0}", message));
		}

		/// <summary>
		/// Writes the value of the specified source taken
		/// from the value of the <see cref="Exception.Source"/>
		/// property to the <see cref="TextWriter"/>.
		/// </summary>
		/// <param name="source">The source of the exception.</param>
		protected override void WriteSource(string source) {
			this.Writer.Write(string.Format("Source: {0}", source));
		}

		/// <summary>
		/// Writes the value of the specified help link taken
		/// from the value of the <see cref="Exception.HelpLink"/>
		/// property to the <see cref="TextWriter"/>.
		/// </summary>
		/// <param name="helpLink">The exception's help link.</param>
		protected override void WriteHelpLink(string helpLink) {
			this.Writer.Write(string.Format("HelpLink: {0}", helpLink));
		}

		/// <summary>
		/// Writes the name and value of the specified property to the <see cref="TextWriter"/>.
		/// </summary>
		/// <param name="propertyInfo">The reflected <see cref="PropertyInfo"/> object.</param>
		/// <param name="value">The value of the <see cref="PropertyInfo"/> object.</param>
		protected override void WritePropertyInfo(PropertyInfo propertyInfo, object value) {
			this.Writer.Write(propertyInfo.Name);
			this.Writer.Write(" : ");
			this.Writer.Write(value);
		}

		/// <summary>
		/// Writes the name and value of the specified field to the <see cref="TextWriter"/>.
		/// </summary>
		/// <param name="fieldInfo">The reflected <see cref="FieldInfo"/> object.</param>
		/// <param name="value">The value of the <see cref="FieldInfo"/> object.</param>
		protected override void WriteFieldInfo(FieldInfo fieldInfo, object value) {
			this.Writer.Write(fieldInfo.Name);
			this.Writer.Write(" : ");
			this.Writer.Write(value);
		}

		/// <summary>
		/// Writes the value of the <see cref="System.Exception.StackTrace"/> property to the <see cref="TextWriter"/>.
		/// </summary>
		/// <param name="stackTrace">The stack trace of the exception.</param>
		/// <remarks>
		/// If there is no stack trace available, an appropriate message will be displayed.
		/// </remarks>
		protected override void WriteStackTrace(string stackTrace) {
			string writeValue = (string.IsNullOrEmpty(stackTrace)) ? "(not available)" : stackTrace;
			this.Writer.Write(string.Format("StackTrace: {0}", writeValue));
		}

		/// <summary>
		/// Writes the additional properties to the <see cref="TextWriter"/>.
		/// </summary>
		/// <param name="additionalInformation">Additional information to be included with the exception report</param>
		protected override void WriteAdditionalInfo(NameValueCollection additionalInformation) {
			this.Writer.Write("AdditionalInfo");

			foreach (string name in additionalInformation.AllKeys) {
				this.Writer.Write(name);
				this.Writer.Write(" : ");
				this.Writer.Write(additionalInformation[name]);
				this.Writer.Write("\n");
			}
		}

		public override void Format() {
			this.WriteException(this.Exception, null);
		}

		protected override void WriteException(Exception exceptionToFormat, Exception outerException) {
			if (exceptionToFormat == null) throw new ArgumentNullException("exceptionToFormat");

			this.WriteMessage(exceptionToFormat.Message);
			this.WriteSpacer();

			if (exceptionToFormat.InnerException != null) {
				this.Writer.Write(string.Format("Inner Exception: {0}", exceptionToFormat.InnerException.Message));
				this.WriteSpacer();
			}

			this.WriteDescription();
			this.WriteSpacer();

			this.WriteStackTrace(exceptionToFormat.StackTrace);
			this.WriteSpacer();

			//this.WriteSource(exceptionToFormat.Source);
			//this.WriteSpacer();

			//this.WriteReflectionInfo(exceptionToFormat);
			//this.WriteSpacer();

			// We only want additional information on the top most exception
			if (outerException == null) {
				this.WriteSpacer();
				this.WriteAdditionalInfo(this.AdditionalInfo);
			}

			Exception inner = exceptionToFormat.InnerException;

			if (inner != null) {
				// recursive call
				this.WriteSpacer();
				this.WriteException(inner, exceptionToFormat);
			}
		}

		protected virtual void WriteSpacer() {
			this.Writer.Write(" -- ");
		}
	}
}