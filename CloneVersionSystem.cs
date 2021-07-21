using System;
using System.Collections.Generic;

namespace Clones
{
	public class CloneVersionSystem : ICloneVersionSystem
	{
		private readonly List<CloneData> _cloneDatabase = new List<CloneData>();

		public CloneVersionSystem()
		{
			_cloneDatabase.Add(new CloneData());
		}
		
		public string Execute(string query)
		{
			var parsedQuery = query.Split();
			var command = parsedQuery[0];
			var cloneNumber = int.Parse(parsedQuery[1]) - 1;
			switch (command)
			{
				case "rollback":
					_cloneDatabase[cloneNumber].Rollback();
					break;
				case "relearn":
					_cloneDatabase[cloneNumber].Relearn();
					break;
				case "check":
					return _cloneDatabase[cloneNumber].Check();
				case "clone":
					var newClone = _cloneDatabase[cloneNumber].Clone();
					_cloneDatabase.Add(newClone);
					break;
				case "learn":
					var skillNumber = int.Parse(parsedQuery[2]);
					_cloneDatabase[cloneNumber].Learn(skillNumber);
					break;
			}
			return null;
		}
	}

	class CloneData
	{
		private readonly Stack<int> _obtainedSkills;
		private readonly Stack<int> _rollbackSkills;
		
		public CloneData()
		{
			_obtainedSkills = new Stack<int>();
			_rollbackSkills = new Stack<int>();
		}

		public CloneData(Stack<int> obtainedSkills, Stack<int> rollbackSkills)
		{
			_obtainedSkills = obtainedSkills;
			_rollbackSkills = rollbackSkills;
		}

		public void Learn(int skillNumber)
		{
			_obtainedSkills.Push(skillNumber);
			_rollbackSkills.Clear();
		}

		public void Rollback()
		{
			var skillNumber = _obtainedSkills.Pop();
			_rollbackSkills.Push(skillNumber);
		}

		public void Relearn()
		{
			var skillNumber = _rollbackSkills.Pop();
			_obtainedSkills.Push(skillNumber);
		}

		public CloneData Clone()
		{
			return new CloneData(_obtainedSkills.Clone(), _rollbackSkills.Clone());
		}

		public string Check()
		{
			if (_obtainedSkills.Count != 0)
				return _obtainedSkills.Peek().ToString();
			return "basic";
		}
	}

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

		private Stack(StackItem<T> head, int count)
		{
			Count = count;
			Head = head;
		}

		public Stack() { }

		public IEnumerator<T> GetEnumerator()
		{
			var current = Head;
			while (current != null)
			{
				yield return current.Value;
				current = current.Previous;
			}
		}
	
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();

		public void Push(T value)
		{
			if (IsEmpty)
				Head = new StackItem<T>(value, null);
			else
				Head = new StackItem<T>(value, Head);
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
	
		public Stack<T> Clone() => new Stack<T>(Head, Count);

		public void Clear()
		{
			Head = null;
		}
	}
}
