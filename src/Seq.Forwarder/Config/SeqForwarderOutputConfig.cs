// Copyright Datalust Pty Ltd
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using Newtonsoft.Json;
using Seq.Forwarder.Cryptography;
using Serilog.Events;

// ReSharper disable UnusedMember.Global, AutoPropertyCanBeMadeGetOnly.Global

namespace Seq.Forwarder.Config
{
    public class SeqForwarderOutputConfig
    {
        public string ServerUrl { get; set; } = "http://localhost:5341";
        public ulong EventBodyLimitBytes { get; set; } = 256 * 1024;
        public ulong RawPayloadLimitBytes { get; set; } = 10 * 1024 * 1024;
        public TimeSpan PooledConnectionLifetime { get; set; } = TimeSpan.FromMinutes(2);

        public string MinimumLevel { get; set; } = "Error";

        const string ProtectedDataPrefix = "pd.";

        public string? ApiKey { get; set; }

        public string? GetApiKey(IStringDataProtector dataProtector)
        {
            if (string.IsNullOrWhiteSpace(ApiKey))
                return null;

            if (!ApiKey.StartsWith(ProtectedDataPrefix))
                return ApiKey;

            return dataProtector.Unprotect(ApiKey.Substring(ProtectedDataPrefix.Length));
        }
        
        public void SetApiKey(string? apiKey, IStringDataProtector dataProtector)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                ApiKey = null;
                return;
            }

            ApiKey = $"{ProtectedDataPrefix}{dataProtector.Protect(apiKey)}";
        }

        private LogEventLevel? _minimumLevel;

        public LogEventLevel GetMinimumLevel() => _minimumLevel ??= (
            string.Equals(MinimumLevel, "off", System.StringComparison.OrdinalIgnoreCase) ? (LogEventLevel)(LevelAlias.Maximum + 1) :
            string.Equals(MinimumLevel, "minimum", System.StringComparison.OrdinalIgnoreCase) ? LevelAlias.Minimum :
            string.Equals(MinimumLevel, "maximum", System.StringComparison.OrdinalIgnoreCase) ? LevelAlias.Maximum :
            string.Equals(MinimumLevel, "verbose", System.StringComparison.OrdinalIgnoreCase) ? LogEventLevel.Verbose :
            string.Equals(MinimumLevel, "debug", System.StringComparison.OrdinalIgnoreCase) ? LogEventLevel.Debug :
            string.Equals(MinimumLevel, "information", System.StringComparison.OrdinalIgnoreCase) ? LogEventLevel.Information :
            string.Equals(MinimumLevel, "warning", System.StringComparison.OrdinalIgnoreCase) ? LogEventLevel.Warning :
            string.Equals(MinimumLevel, "error", System.StringComparison.OrdinalIgnoreCase) ? LogEventLevel.Error :
            string.Equals(MinimumLevel, "fatal", System.StringComparison.OrdinalIgnoreCase) ? LogEventLevel.Fatal :
            LogEventLevel.Information);
    }
}
