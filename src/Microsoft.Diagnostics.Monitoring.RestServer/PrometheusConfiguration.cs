﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace Microsoft.Diagnostics.Monitoring.RestServer
{
    /// <summary>
    /// Configuration for prometheus metric collection and retrieval.
    /// TODO We may want to expose https endpoints here as well, and make port changes
    /// TODO How do we determine which process to scrape in multi-proc situations? How do we configure this
    /// for situations where the pid is not known or ambiguous?
    /// </summary>
    public class PrometheusConfiguration
    {
        private readonly Lazy<int?[]> _ports;
        
        public PrometheusConfiguration()
        {
            _ports = new Lazy<int?[]>(() =>
                {
                    string[] endpoints = Endpoints.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    int?[] ports = new int?[endpoints.Length];
                    for(int i = 0; i < endpoints.Length; i++)
                    {
                        //We cannot use Uri[Builder], since we are sometimes parsing invalid hostnames
                        try
                        {
                            UriHelper.FromAbsolute(endpoints[i], out _, out HostString host, out _, out _, out _);
                            ports[i] = host.Port;
                        }
                        catch (FormatException)
                        {
                        }
                    }
                    return ports;
                });
        }

        public bool Enabled { get; set; }
        
        public string Endpoints { get; set; }

        public int?[] Ports => _ports.Value;

        public int UpdateIntervalSeconds { get; set; }

        public int MetricCount { get; set; }
    }
}
