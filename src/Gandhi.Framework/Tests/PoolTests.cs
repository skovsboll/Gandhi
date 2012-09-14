using FluentAssertions;
using Ghandi.Love;
using Xunit;

namespace Ghandi.Tests
{
	public class PoolTests
	{

		[Fact]
		public void Pool_produces_leases_with_different_ids()
		{
			var sut = new Pool<string>(() => "42");

			using (var lease1 = sut.GetLease())
			using (var lease2 = sut.GetLease())
			{
				lease1.Id.Should().NotBe(lease2.Id);
			}
		}

		[Fact]
		public void Pool_can_reuse_item()
		{
			var sut = new Pool<string>(() => "42");

			using (var lease = sut.GetLease())
			{
				lease.Value.Should().Be("42");
			}

			using (var lease = sut.GetLease())
			{
				lease.Value.Should().Be("42");
			}

			sut.ActiveLeasesCount.Should().Be(0);
			sut.AvailableLeasesCount.Should().Be(1);
		}

		[Fact]
		public void Pool_can_be_compacted()
		{
			var sut = new Pool<string>(() => "42");

			using (var lease1 = sut.GetLease())
			using (var lease2 = sut.GetLease())
			using (var lease3 = sut.GetLease())
			{
				sut.ActiveLeasesCount.Should().Be(3);
				sut.AvailableLeasesCount.Should().Be(0);				
			}

			sut.ActiveLeasesCount.Should().Be(0);
			sut.AvailableLeasesCount.Should().Be(3);

			sut.Compact();

			sut.ActiveLeasesCount.Should().Be(0);
			sut.AvailableLeasesCount.Should().Be(0);
		}

		[Fact]
		public void Pool_compaction_only_affects_available_leases()
		{
			var sut = new Pool<string>(() => "42");

			using (var lease1 = sut.GetLease())
			using (var lease2 = sut.GetLease())
			using (var lease3 = sut.GetLease())
			{
				sut.ActiveLeasesCount.Should().Be(3);
				sut.AvailableLeasesCount.Should().Be(0);

				sut.Compact();
	
				sut.ActiveLeasesCount.Should().Be(3);
				sut.AvailableLeasesCount.Should().Be(0);
			}
		}

	}
}