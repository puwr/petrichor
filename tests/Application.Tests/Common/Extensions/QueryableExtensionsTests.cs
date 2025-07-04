using FluentAssertions;
using Application.Common.Extensions;
using Contracts.Pagination;
using TestUtilities.Database;
using Infrastructure.Persistence;
using Domain.Images;
using TestUtilities.Images;

namespace Application.Tests.Common.Extensions;

public class QueryableExtensionsTests
{
    private readonly List<int> _testNumbers = Enumerable.Range(1, 3).ToList();

    [Fact]
    public void WhereIf_AppliesPredicate()
    {
        var filtered = _testNumbers.AsQueryable().WhereIf(
            condition: true,
            predicate: x => x > 1);

        filtered.ToList().Should().BeEquivalentTo(_testNumbers.Where(n => n > 1));
    }

    [Fact]
    public void WhereIf_WhenConditionIsFalse_ReturnsOriginal()
    {
        var filtered = _testNumbers.AsQueryable().WhereIf(
            condition: false,
            predicate: x => x > 1);

        filtered.ToList().Should().BeEquivalentTo(_testNumbers);
    }

    [Trait("Category", "Integration")]
    public class DatabaseTests : IDisposable
    {
        private readonly TestDatabase _testDatabase;
        private readonly PetrichorDbContext _dbContext;

        public DatabaseTests()
        {
            _testDatabase = TestDatabase.CreateDatabase();
            _dbContext = _testDatabase.CreateDbContext();
        }

        [Fact]
        public async Task ToPagedResponseAsync_ReturnsPagedResponse()
        {
            const int pageNumber = 2;
            const int pageSize = 5;
            const int totalCount = 9;

            SeedTestData(_dbContext, count: totalCount);
            var pagination = new PaginationParameters(pageNumber, pageSize);
            var query = _dbContext.Images.AsQueryable();

            var pagedResponse = await query.ToPagedResponseAsync(pagination);

            pagedResponse.Should().NotBeNull();
            pagedResponse.Should().BeOfType<PagedResponse<Image>>();
            pagedResponse.Items.Should().AllBeOfType<Image>();
            // 2 page of 9 items (5 per page) should return 4 last items
            pagedResponse.Items.Count.Should().Be(4);
            pagedResponse.Count.Should().Be(totalCount);
            pagedResponse.PageNumber.Should().Be(pageNumber);
            pagedResponse.PageSize.Should().Be(pageSize);
            pagedResponse.TotalPages.Should().Be(2);
        }

        [Fact]
        public async Task ToPagedResponseAsync_WhenTableIsEmpty_ReturnsEmptyPagedResponse()
        {
            const int pageNumber = 1;
            const int pageSize = 5;

            var pagination = new PaginationParameters(pageNumber, pageSize);
            var query = _dbContext.Images.AsQueryable();

            var pagedResponse = await query.ToPagedResponseAsync(pagination);

            pagedResponse.Should().NotBeNull();
            pagedResponse.Should().BeOfType<PagedResponse<Image>>();
            pagedResponse.Items.Should().BeEmpty();
            pagedResponse.Count.Should().Be(0);
            pagedResponse.PageNumber.Should().Be(pageNumber);
            pagedResponse.PageSize.Should().Be(pageSize);
            pagedResponse.TotalPages.Should().Be(0);
        }

        private static List<Image> SeedTestData(PetrichorDbContext dbContext, int count = 1)
        {
            var testImages = ImageFactory.CreateImages(count);

            dbContext.Images.AddRange(testImages);
            dbContext.SaveChanges();

            return testImages;
        }

        public void Dispose()
        {
            _dbContext.Dispose();
            _testDatabase.Dispose();
        }
    }
}
