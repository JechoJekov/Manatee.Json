﻿using System;
using System.IO;
using System.Net;
using Manatee.Json.Serialization;

#if !NET45
using System.Net.Http;
#endif

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Defines options associated with JSON Schema.
	/// </summary>
	public static class JsonSchemaOptions
	{
		private static Func<string, string> _download;

		/// <summary>
		/// Gets and sets a method used to download online schema.
		/// </summary>
		public static Func<string, string> Download
		{
			get { return _download ?? (_download = _BasicDownload); }
			set { _download = value; }
		}

		/// <summary>
		/// Gets or sets whether the "format" schema keyword should be validated.  The default is true.
		/// </summary>
		public static bool ValidateFormatKeyword { get; set; } = true;

		/// <summary>
		/// Gets or sets whether unknown string formats are permitted.  If disabled and an unknown format
		/// is found, the system will throw a <see cref="JsonSerializationException"/> while loading the schema.
		/// The default is true.
		/// </summary>
		public static bool AllowUnknownFormats { get; set; } = true;

		/// <summary>
		/// Gets or sets the output verbosity.  The default is <see cref="SchemaValidationOutputFormat.Detailed"/>.
		/// </summary>
		public static SchemaValidationOutputFormat OutputFormat { get; set; } = SchemaValidationOutputFormat.Detailed;

		/// <summary>
		/// Determines how `$ref` keywords are resolved when adjacent to an `$id` keyword
		/// when a specific draft cannot be identified.
		/// </summary>
		/// <remarks>
		/// As of draft-08, keywords are allowed to be adjacent to `$ref`.  This means that an
		/// adjacent `$id` keyword will now change the base URI whereas in prior drafts it would not
		/// since adjacent keywords were to be ignored.
		///
		/// When Manatee.Json cannot determine the draft a particular schema is using (determined via
		/// the `$schema` keyword or the selection of keywords being used), this option will
		/// determine the behavior for resolving URIs.
		/// </remarks>
		public static RefResolutionStrategy RefResolution { get; set; } = RefResolutionStrategy.IgnoreSiblingId;

		private static string _BasicDownload(string path)
		{
			Console.WriteLine(path);
			var uri = new Uri(path);

			switch (uri.Scheme)
			{
				case "http":
				case "https":
#if NET45
					return new WebClient().DownloadString(uri);
#else
					return new HttpClient().GetStringAsync(uri).Result;
#endif
				case "file":
					var filename = Uri.UnescapeDataString(uri.AbsolutePath);
					return File.ReadAllText(filename);
				default:
					throw new Exception($"URI scheme {uri.Scheme} is not supported.  Only HTTP(S) and local file system URIs are allowed.");
			}
		}
	}

	/// <summary>
	/// Determines how `$ref` keywords are resolved when adjacent to an
	/// `$id` keyword.
	/// </summary>
	/// <remarks>
	/// See the specific members for examples.
	/// </remarks>
	public enum RefResolutionStrategy
	{
		/// <summary>
		/// Sibling `$id` properties will be ignored and will not change the
		/// base URI.  This will process `$ref` according to draft-07 and earlier.
		/// </summary>
		/// <example>
		/// The `$ref` in the following schema will resolve to <i>http://example.com/document.json</i>
		/// because the `$id` of <i>folder</i> will be ignored.
		/// 
		/// `
		/// {
		///   "$schema": "http://json-schema.org/draft-08/schema#",
		///   "$id": "http://example.com/root.json"
		///   "properties": {
		///     "prop": {
		///       "$id": "folder"
		///       "$ref": "document.json"
		///     }
		///   }
		/// }
		/// `
		/// </example>
		IgnoreSiblingId,
		/// <summary>
		/// Sibling `$id` properties will be processed and will change the
		/// base URI.  This will process `$ref` according to draft-08.
		/// </summary>
		/// <example>
		/// The `$ref` in the following schema will resolve to <i>http://example.com/folder/document.json</i>
		/// because the `$id` of <i>folder</i> will be processed.
		/// 
		/// `
		/// {
		///   "$schema": "http://json-schema.org/draft-08/schema#",
		///   "$id": "http://example.com/root.json"
		///   "properties": {
		///     "prop": {
		///       "$id": "folder"
		///       "$ref": "document.json"
		///     }
		///   }
		/// }
		/// `
		/// </example>
		ProcessSiblingId
	}
}
