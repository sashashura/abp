﻿using System.Threading.Tasks;
using Shouldly;
using Volo.CmsKit.Tags;
using Xunit;

namespace Volo.CmsKit.Pages;

public class PageManager_Test : CmsKitDomainTestBase
{
	private readonly PageManager pageManager;
	private readonly CmsKitTestData testData;
	private readonly IPageRepository pageRepository;

	public PageManager_Test()
	{
		pageManager = GetRequiredService<PageManager>();
		testData = GetRequiredService<CmsKitTestData>();
		pageRepository = GetRequiredService<IPageRepository>();
	}

	[Fact]
	public async Task CreateAsync_ShouldWorkProperly_WithNonExistingSlug()
	{
		var title = "My awesome page";
		var slug = "my-awesome-page";
		var content = "<h1>My Awesome Page</h1><p>This is my awesome page content!</p>";

		var page = await pageManager.CreateAsync(title, slug, content);

		page.ShouldNotBeNull();
		page.Title.ShouldBe(title);
		page.Slug.ShouldBe(slug);
		page.Content.ShouldBe(content);
	}

	[Fact]
	public async Task CreateAsync_ShouldThrowException_WithExistingSlug()
	{
		var title = "My awesome page";
		var slug = testData.Page_1_Slug;
		var content = "<h1>My Awesome Page</h1><p>This is my awesome page content!</p>";

		var exception = await Should.ThrowAsync<PageSlugAlreadyExistsException>(async () =>
							await pageManager.CreateAsync(title, slug, content));

		exception.ShouldNotBeNull();
	}

	[Fact]
	public async Task SetSlugAsync_ShouldWorkProperly_WithNonExistingSlug()
	{
		var newSlug = "freshly-generated-new-slug";
		var page = await pageRepository.GetAsync(testData.Page_1_Id);

		await pageManager.SetSlugAsync(page, newSlug);

		page.Slug.ShouldBe(newSlug);
	}

	[Fact]
	public async Task SetSlugAsync_ShouldThrowException_WithExistingSlug()
	{
		var newSlug = testData.Page_2_Slug;
		var page = await pageRepository.GetAsync(testData.Page_1_Id);

		var exception = await Should.ThrowAsync<PageSlugAlreadyExistsException>(async () =>
							await pageManager.SetSlugAsync(page, newSlug));

		exception.ShouldNotBeNull();
	}

	[Fact]
	public async Task SetHomePageAsync_ShouldWorkProperly_IfExistHomePage()
	{
		await WithUnitOfWorkAsync(async ()=>
		{
			var page = await pageRepository.GetAsync(testData.Page_1_Id);

			await pageManager.SetHomePageAsync(page);
		});

		var page = await pageRepository.GetAsync(testData.Page_1_Id);
		page.IsHomePage.ShouldBeTrue();

		var pageSetAsHomePageAsFalse = await pageRepository.GetAsync(testData.Page_2_Id);
		pageSetAsHomePageAsFalse.IsHomePage.ShouldBeFalse();
	}

	[Fact]
	public async Task SetHomePageAsync_ShouldFix_WhenMultipleHomePageExist()
	{
		await WithUnitOfWorkAsync(async () =>
		{
			var page1 = await pageRepository.GetAsync(testData.Page_1_Id);
			var page2 = await pageRepository.GetAsync(testData.Page_2_Id);

			page1.SetIsHomePage(true);
			page2.SetIsHomePage(true);

			await pageRepository.UpdateManyAsync(new[] { page1, page2 }, autoSave: true);

			await pageManager.SetHomePageAsync(page1);
		});

		var page1 = await pageRepository.GetAsync(testData.Page_1_Id);
		var page2 = await pageRepository.GetAsync(testData.Page_2_Id);

		page2 = await pageRepository.GetAsync(testData.Page_2_Id);

		page1.IsHomePage.ShouldBeTrue();
		page2.IsHomePage.ShouldBeFalse();
	}
}
