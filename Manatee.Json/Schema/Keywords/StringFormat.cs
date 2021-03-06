﻿﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Manatee.Json.Internal;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Defines various string formatting types used for StringSchema validation.
	/// </summary>
	/// <remarks>
	/// Any of these may be overridden by creating a new instance with the same key.
	/// </remarks>
	public class StringFormat
	{
		/// <summary>
		/// Defines an ISO 8601 date format.
		/// </summary>
		public static StringFormat Date { get; set; }
		/// <summary>
		/// Defines an ISO 8601 date/time format.
		/// </summary>
		public static StringFormat DateTime { get; set; }
		/// <summary>
		/// Defines an ISO 8601 duration format.
		/// </summary>
		public static StringFormat Duration { get; set; }
		/// <summary>
		/// Defines an email address format.
		/// </summary>
		/// <remarks>
		/// From http://www.regular-expressions.info/email.html
		/// </remarks>
		public static StringFormat Email { get; set; }
		// from [lost the link, sorry]
		/// <summary>
		/// Defines a host name format.
		/// </summary>
		public static StringFormat HostName { get; set; }
		// from [lost the link, sorry]
		/// <summary>
		/// Defines an IPV4 address format.
		/// </summary>
		public static StringFormat Ipv4 { get; set; }
		// from [lost the link, sorry]
		/// <summary>
		/// Defines an IPV6 format.
		/// </summary>
		public static StringFormat Ipv6 { get; set; }
		/// <summary>
		/// Defines an IRI format via <see cref="System.Uri.IsWellFormedUriString(string, UriKind)"/>.
		/// </summary>
		public static StringFormat Iri { get; set; }
		/// <summary>
		/// Defines an IRI Reference format via <see cref="System.Uri.IsWellFormedUriString(string, UriKind)"/>.
		/// </summary>
		public static StringFormat IriReference { get; set; }
		/// <summary>
		/// Defines a JSON Pointer format.
		/// </summary>
		public static StringFormat JsonPointer { get; set; }
		/// <summary>
		/// Defines a regular expression format.
		/// </summary>
		public static StringFormat Regex { get; set; }
		/// <summary>
		/// Defines a Relative JSON Pointer format.
		/// </summary>
		public static StringFormat RelativeJsonPointer { get; set; }
		/// <summary>
		/// Defines a URI format via <see cref="System.Uri.IsWellFormedUriString(string, UriKind)"/>.
		/// </summary>
		/// <remarks>For draft-06 schema and later, only use this for absolute URIs.</remarks>
		public static StringFormat Uri { get; set; }
		/// <summary>
		/// Defines a URI Reference format via <see cref="System.Uri.IsWellFormedUriString(string, UriKind)"/>.
		/// </summary>
		public static StringFormat UriReference { get; set; }
		/// <summary>
		/// Defines a URI Template format via <see cref="System.Uri.IsWellFormedUriString(string, UriKind)"/>.
		/// </summary>
		public static StringFormat UriTemplate { get; set; }
		/// <summary>
		/// Defines a UUID format.
		/// </summary>
		public static StringFormat Uuid { get; set; }

		private static readonly Dictionary<string, StringFormat> _lookup;

		/// <summary>
		/// A string key which specifies this string format.
		/// </summary>
		public string Key { get; }
		
		/// <summary>
		/// Gets
		/// </summary>
		public JsonSchemaVersion SupportedBy { get; }

		/// <summary>
		/// If created with a regular expression, this gets the regular expression.
		/// </summary>
		public Regex ValidationRegex { get; }

		/// <summary>
		/// If created with a validation function, this gets the function.
		/// </summary>
		public Func<string, bool> ValidationFunction { get; }

		internal bool IsKnown { get; private set; } = true;

		private static string[] DateTimeFormats = new[]
		{
			"yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffffffK",
			"yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'ffffffK",
			"yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffffK",
			"yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'ffffK",
			"yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffK",
			"yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'ffK",
			"yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fK",
			"yyyy'-'MM'-'dd'T'HH':'mm':'ssK",
			"yyyy'-'MM'-'dd'T'HH':'mm':'ss",
		};

		static StringFormat()
		{
			_lookup = new Dictionary<string, StringFormat>();

			Date = new StringFormat("date", JsonSchemaVersion.Draft2019_06,
			                        @"^(\d{4})-(\d{2})-(\d{2})$");
			DateTime = new StringFormat("date-time", JsonSchemaVersion.All,
										s => System.DateTimeOffset.TryParseExact(s, DateTimeFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out _));
			Duration = new StringFormat("duration", JsonSchemaVersion.Draft2019_06,
			                            @"^(-?)P(?=\d|T\d)(?:(\d+)Y)?(?:(\d+)M)?(?:(\d+)([DW]))?(?:T(?:(\d+)H)?(?:(\d+)M)?(?:(\d+(?:\.\d+)?)S)?)?$");
			Email = new StringFormat("email", JsonSchemaVersion.All,
			                         @"^(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|""(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*"")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\])$");
			HostName = new StringFormat("hostname", JsonSchemaVersion.All,
			                            @"^(?!.{255,})([a-zA-Z0-9-]{0,63}\.)*([a-zA-Z0-9-]{0,63})$");
			Ipv4 = new StringFormat("ipv4", JsonSchemaVersion.All,
			                        @"^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$");
			Ipv6 = new StringFormat("ipv6", JsonSchemaVersion.All,
			                        @"^(([0-9a-fA-F]{1,4}:){7,7}[0-9a-fA-F]{1,4}|([0-9a-fA-F]{1,4}:){1,7}:|([0-9a-fA-F]{1,4}:){1,6}:[0-9a-fA-F]{1,4}|([0-9a-fA-F]{1,4}:){1,5}(:[0-9a-fA-F]{1,4}){1,2}|([0-9a-fA-F]{1,4}:){1,4}(:[0-9a-fA-F]{1,4}){1,3}|([0-9a-fA-F]{1,4}:){1,3}(:[0-9a-fA-F]{1,4}){1,4}|([0-9a-fA-F]{1,4}:){1,2}(:[0-9a-fA-F]{1,4}){1,5}|[0-9a-fA-F]{1,4}:((:[0-9a-fA-F]{1,4}){1,6})|:((:[0-9a-fA-F]{1,4}){1,7}|:)|fe80:(:[0-9a-fA-F]{0,4}){0,4}%[0-9a-zA-Z]{1,}|::(ffff(:0{1,4}){0,1}:){0,1}((25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9]).){3,3}(25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])|([0-9a-fA-F]{1,4}:){1,4}:((25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9]).){3,3}(25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9]))$");
			IriReference = new StringFormat("iri-reference", JsonSchemaVersion.Draft2019_06,
			                                @"^(([^:/?#]+):)?(//([^/?#]*))?([^?#]*)(\?([^#]*))?(#(.*))?");
			Iri = new StringFormat("iri", JsonSchemaVersion.Draft2019_06, s => System.Uri.IsWellFormedUriString(s, UriKind.RelativeOrAbsolute));
			JsonPointer = new StringFormat("json-pointer", JsonSchemaVersion.Draft2019_06, @"^(/(([^/~])|(~[01]))+)*/?$");
			Regex = new StringFormat("regex", JsonSchemaVersion.All, null, true);
			RelativeJsonPointer = new StringFormat("relative-json-pointer", JsonSchemaVersion.Draft2019_06, @"^[0-9]+#/(([^/~])|(~[01]))*$");
			UriReference = new StringFormat("uri-reference", JsonSchemaVersion.Draft06 | JsonSchemaVersion.Draft07 | JsonSchemaVersion.Draft2019_06,
			                                @"^(([^:/?#]+):)?(//([^/?#]*))?([^?#]*)(\?([^#]*))?(#(.*))?");
			UriTemplate = new StringFormat("uri-template", JsonSchemaVersion.Draft2019_06,
			                                @"^$");
			Uri = new StringFormat("uri", JsonSchemaVersion.All, s => System.Uri.IsWellFormedUriString(s, UriKind.RelativeOrAbsolute));
			Uuid = new StringFormat("uuid", JsonSchemaVersion.Draft2019_06,
			                        @"[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}");
		}
		/// <summary>
		/// Creates a new <see cref="StringFormat"/> instance using a regular expression.
		/// </summary>
		/// <param name="key">The name of the format.  This is the value that will appear in the schema.</param>
		/// <param name="supportedBy">The schema drafts supported by this keyword.</param>
		/// <param name="regex">The validation regular expression.</param>
		/// <param name="isCaseSensitive">Whether the regular expression is to be processed as case-sensitive.</param>
		public StringFormat(string key, JsonSchemaVersion supportedBy, [RegexPattern] string regex, bool isCaseSensitive = false)
		{
			Key = key;
			SupportedBy = supportedBy;
			if (regex != null)
				ValidationRegex = new Regex(regex, isCaseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase);

			_lookup.Add(key, this);
		}
		/// <summary>
		/// Creates a new <see cref="StringFormat"/> instance using a validation function.
		/// </summary>
		/// <param name="key">The name of the format.  This is the value that will appear in the schema.</param>
		/// <param name="supportedBy">The schema drafts supported by this keyword.</param>
		/// <param name="validate">The function by which the string is validated.</param>
		public StringFormat(string key, JsonSchemaVersion supportedBy, Func<string, bool> validate)
		{
			ValidationFunction = validate;
			Key = key;
			SupportedBy = supportedBy;

			_lookup.Add(key, this);
		}

		/// <summary>
		/// Validates a value to the specified format.
		/// </summary>
		/// <param name="value">The value to validate.</param>
		/// <returns>True if the value is valid, otherwise false.</returns>
		public bool Validate(string value)
		{
			if (ValidationRegex == null)
			{
				return ValidationFunction == null || ValidationFunction(value);
			}
			return ValidationRegex.IsMatch(value);
		}

		/// <summary>
		/// Gets a <see cref="StringFormat"/> object based on a format key.
		/// </summary>
		/// <param name="formatKey">The predefined key for the format.</param>
		/// <returns>A <see cref="StringFormat"/> object, or null if none exists for the key.</returns>
		public static StringFormat GetFormat(string formatKey)
		{
			return formatKey != null && _lookup.ContainsKey(formatKey)
				? _lookup[formatKey]
				: new StringFormat(formatKey, JsonSchemaVersion.None, s => JsonSchemaOptions.AllowUnknownFormats) {IsKnown = false};
		}
	}
}