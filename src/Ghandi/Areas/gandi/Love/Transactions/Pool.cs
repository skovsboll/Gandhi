using System;
using System.Collections.Concurrent;

namespace Ghandi.Love
{
	public class Pool<T>
	{
		private readonly Func<T> _create;
		private readonly Action<T> _prepare;
		private readonly Action<T> _dismantle;

		readonly ConcurrentDictionary<Guid, Lease> _activeLeases = new ConcurrentDictionary<Guid, Lease>();
		readonly ConcurrentBag<Lease> _availableLeases = new ConcurrentBag<Lease>();

		public Pool()
			: this(() => default(T))
		{
		}

		public Pool(Func<T> create)
			: this(create, t => { })
		{
		}

		public Pool(Func<T> create, Action<T> prepare)
			: this(create, prepare, t => { })
		{
		}

		public Pool(Func<T> create, Action<T> prepare, Action<T> dismantle)
		{
			_dismantle = dismantle;
			_prepare = prepare;
			_create = create;
		}

		public class Lease : IDisposable
		{
			internal Lease(Pool<T> pool, T value)
			{
				_pool = pool;
				_value = value;
				_id = Guid.NewGuid();
			}

			private readonly T _value;
			private readonly Pool<T> _pool;
			private readonly Guid _id;
			public Guid Id { get { return _id; } }
			public T Value { get { return _value; } }

			public void Dispose()
			{
				_pool.Release(this);
			}
		}

		public int ActiveLeasesCount
		{
			get { return _activeLeases.Count; }
		}

		public int AvailableLeasesCount
		{
			get { return _availableLeases.Count; }
		}

		public Lease GetLease()
		{
			Lease lease;

			if (_availableLeases.TryTake(out lease))
			{
				if (!_activeLeases.TryAdd(lease.Id, lease))
					throw new InvalidOperationException("Same lease in use twice. Not good.");			

				_prepare(lease.Value);
				return lease;
			}

			lease = new Lease(this, _create());
			if (_activeLeases.TryAdd(lease.Id, lease))
				return lease;

			throw new InvalidOperationException("Same lease in use twice. Not good.");
		}

		public void Release(Lease lease)
		{
			if (!_activeLeases.TryRemove(lease.Id, out lease))
				throw new InvalidOperationException("Same lease released twice. Not good.");

			_availableLeases.Add(lease);
		}

		public void Compact()
		{
			Lease lease;
			while (_availableLeases.TryTake(out lease))
			{
				_dismantle(lease.Value);
			}
		}
	}
}