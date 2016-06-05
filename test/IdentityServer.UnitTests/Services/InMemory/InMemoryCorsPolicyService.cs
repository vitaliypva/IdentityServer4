﻿
using FluentAssertions;
using IdentityServer4.Core.Models;
using IdentityServer4.Core.Services.InMemory;
using System.Collections.Generic;
using UnitTests.Common;
using Xunit;

namespace UnitTests.Services.InMemory
{
    public class InMemoryCorsPolicyServiceTests
    {
        const string Category = "InMemoryCorsPolicyService";

        InMemoryCorsPolicyService _subject;
        List<Client> _clients = new List<Client>();

        public InMemoryCorsPolicyServiceTests()
        {
            _subject = new InMemoryCorsPolicyService(new FakeLogger<InMemoryCorsPolicyService>(), _clients);
        }

        [Fact]
        [Trait("Category", Category)]
        public void client_has_origin_should_allow_origin()
        {
            _clients.Add(new Client
            {
                AllowedCorsOrigins = new List<string>
                {
                    "http://foo"
                }
            });

            _subject.IsOriginAllowedAsync("http://foo").Result.Should().BeTrue();
        }

        [Theory]
        [InlineData("http://foo")]
        [InlineData("https://bar")]
        [InlineData("http://bar-baz")]
        [Trait("Category", Category)]
        public void client_does_not_has_origin_should_not_allow_origin(string clientOrigin)
        {
            _clients.Add(new Client
            {
                AllowedCorsOrigins = new List<string>
                {
                    clientOrigin
                }
            });
            _subject.IsOriginAllowedAsync("http://bar").Result.Should().Be(false);
        }

        [Fact]
        [Trait("Category", Category)]
        public void client_has_many_origins_and_origin_is_in_list_should_allow_origin()
        {
            _clients.Add(new Client
            {
                AllowedCorsOrigins = new List<string>
                {
                    "http://foo",
                    "http://bar",
                    "http://baz",
                }
            });
            _subject.IsOriginAllowedAsync("http://bar").Result.Should().Be(true);
        }

        [Fact]
        [Trait("Category", Category)]
        public void client_has_many_origins_and_origin_is_in_not_list_should_not_allow_origin()
        {
            _clients.Add(new Client
            {
                AllowedCorsOrigins = new List<string>
                {
                    "http://foo",
                    "http://bar",
                    "http://baz",
                }
            });
            _subject.IsOriginAllowedAsync("http://quux").Result.Should().Be(false);
        }

        [Fact]
        [Trait("Category", Category)]
        public void many_clients_have_same_origins_should_allow_origin()
        {
            _clients.AddRange(new Client[] {
                new Client
                {
                    AllowedCorsOrigins = new List<string>
                    {
                        "http://foo",
                    }
                },
                new Client
                {
                    AllowedCorsOrigins = new List<string>
                    {
                        "http://foo",
                    }
                }
            });
            _subject.IsOriginAllowedAsync("http://foo").Result.Should().BeTrue();
        }
    }
}