﻿using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Volo.Abp.Auditing;

public class JsonAuditSerializer_Test : AbpAuditingTestBase
{
    protected override void AfterAddApplication(IServiceCollection services)
    {
        services.Configure<AbpAuditingOptions>(options =>
        {
            options.IgnoredTypes.Add(typeof(DateTime));
        });

        base.AfterAddApplication(services);
    }

    [Fact]
    public void Serialize_Test()
    {
        var arguments = new Dictionary<string, object>
            {
                {"input", new InputDto {PersonData = "IdCard:123123"}},
                {"input2", new Input2Dto {UserName = "admin", Password = "1q2w3E*", Birthday = DateTime.Now}}
            };

        var str = GetRequiredService<JsonAuditSerializer>().Serialize(arguments);

        str.ShouldNotContain("IdCard");
        str.ShouldNotContain("1q2w3E*");
        str.ShouldNotContain("Birthday");

        str.ShouldContain("UserName");
        str.ShouldContain("admin");
    }


    [DisableAuditing]
    class InputDto
    {
        public string PersonData { get; set; }
    }

    class Input2Dto
    {
        public string UserName { get; set; }

        [DisableAuditing]
        public string Password { get; set; }

        [JsonIgnore]
        public string PrivateEmail { get; set; }

        public DateTime Birthday { get; set; }
    }
}
