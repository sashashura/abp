// This file is automatically generated by ABP framework to use MVC Controllers from CSharp
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Http.Client;
using Volo.Abp.Http.Modeling;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Http.Client.ClientProxying;
using Volo.CmsKit.Admin.GlobalResources;

// ReSharper disable once CheckNamespace
namespace Volo.CmsKit.Admin.GlobalResources.ClientProxies;

[Dependency(ReplaceServices = true)]
[ExposeServices(typeof(IGlobalResourceAdminAppService), typeof(GlobalResourceAdminClientProxy))]
public partial class GlobalResourceAdminClientProxy : ClientProxyBase<IGlobalResourceAdminAppService>, IGlobalResourceAdminAppService
{
    public virtual async Task<GlobalResourcesDto> GetAsync()
    {
        return await RequestAsync<GlobalResourcesDto>(nameof(GetAsync));
    }

    public virtual async Task SetGlobalStyleAsync(GlobalResourceUpdateDto input)
    {
        await RequestAsync(nameof(SetGlobalStyleAsync), new ClientProxyRequestTypeValue
        {
            { typeof(GlobalResourceUpdateDto), input }
        });
    }

    public virtual async Task SetGlobalScriptAsync(GlobalResourceUpdateDto input)
    {
        await RequestAsync(nameof(SetGlobalScriptAsync), new ClientProxyRequestTypeValue
        {
            { typeof(GlobalResourceUpdateDto), input }
        });
    }
}