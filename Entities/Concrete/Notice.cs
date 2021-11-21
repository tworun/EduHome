﻿using Core.Entities.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Concrete
{
	public class Notice:IEntity
	{
		public int Id { get; set; }
		public DateTime Date { get; set; }
		public string Content { get; set; }
	}
}
