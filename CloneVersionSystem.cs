using System;
using System.Collections.Generic;

namespace Clones
{
	public class CloneVersionSystem : ICloneVersionSystem
	{
		private readonly List<CloneData> CloneDatabase;

		public CloneVersionSystem()
		{
			CloneDatabase = new List<CloneData>();
			CloneDatabase.Add(new CloneData());
		}
		
		public string Execute(string query)
		{
			var parsedQuery = query.Split();
			var command = parsedQuery[0];
			var cloneNumber = int.Parse(parsedQuery[1]) - 1;
			switch (command)
			{
				case "rollback":
					CloneDatabase[cloneNumber].Rollback();
					break;
				case "relearn":
					CloneDatabase[cloneNumber].Relearn();
					break;
				case "check":
					return CloneDatabase[cloneNumber].Check();
				case "clone":
					var newClone = CloneDatabase[cloneNumber].Clone();
					CloneDatabase.Add(newClone);
					break;
				case "learn":
					var skillNumber = int.Parse(parsedQuery[2]);
					CloneDatabase[cloneNumber].Learn(skillNumber);
					break;
			}
			return null;
		}
	}

	class CloneData
	{
		private readonly Stack<int> ObtainedSkills;
		private readonly Stack<int> RollbackSkills;
		
		public CloneData()
		{
			ObtainedSkills = new Stack<int>();
			RollbackSkills = new Stack<int>();
		}

		public CloneData(Stack<int> obtainedSkills, Stack<int> rollbackSkills)
		{
			ObtainedSkills = obtainedSkills;
			RollbackSkills = rollbackSkills;
		}

		public void Learn(int skillNumber)
		{
			ObtainedSkills.Push(skillNumber);
			RollbackSkills.Clear();
		}

		public void Rollback()
		{
			var skillNumber = ObtainedSkills.Pop();
			RollbackSkills.Push(skillNumber);
		}

		public void Relearn()
		{
			var skillNumber = RollbackSkills.Pop();
			ObtainedSkills.Push(skillNumber);
		}

		public CloneData Clone()
		{
			return new CloneData(ObtainedSkills.Clone(), RollbackSkills.Clone());
		}

		public string Check()
		{
			if (ObtainedSkills.Count != 0)
				return ObtainedSkills.Peek().ToString();
			return "basic";
		}
	}
	
	// public static class StackExtensions
	// {
	// 	public static Stack<T> Clone<T>(this Stack<T> original)
	// 	{
	// 		var arr = new T[original.Count];
	// 		original.CopyTo(arr, 0);
	// 		Array.Reverse(arr);
	// 		return new Stack<T>(arr);
	// 	}
	// }

	class StackItem<T>
	{
		public T Value { get; }
		public StackItem<T> Previous { get; }
	
		public StackItem(T value, StackItem<T> previous)
		{
			Value = value;
			Previous = previous;
		}
	}
	
	public class Stack<T> : IEnumerable<T>
	{
		public int Count { get; private set; }
		StackItem<T> Head { get; set; }
		public bool IsEmpty => Head == null;

		// private Stack(IEnumerable<T> collection)
		// {
		// 	foreach (var e in collection)
		// 		this.Push(e);
		// }
		
		private Stack(StackItem<T> head, int count)
		{
			Count = count;
			Head = head;
		}

		public Stack()
		{
		}

		public IEnumerator<T> GetEnumerator()
		{
			var current = Head;
			while (current != null)
			{
				yield return current.Value;
				current = current.Previous;
			}
		}
	
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void Push(T value)
		{
			if (IsEmpty)
				Head = new StackItem<T>(value, null);
			else
			{
				var item = new StackItem<T>(value, Head);
				Head = item;
			}
			Count++;
		}
	
		public T Pop()
		{
			if (Head == null) throw new InvalidOperationException();
			var result = Head.Value;
			Head = Head.Previous;
			Count--;
			return result;
		}
	
		public T Peek()
		{
			if (Head == null) throw new InvalidOperationException();
			return Head.Value;
		}
	
		public Stack<T> Clone()
		{
			return new Stack<T>(Head, Count);
		}
	
		public void Clear()
		{
			Head = null;
		}
	}
}
