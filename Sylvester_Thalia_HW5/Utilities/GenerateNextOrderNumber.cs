using System;
using Sylvester_Thalia_HW5.DAL;
namespace Sylvester_Thalia_HW5.Utilities
{
	public class GenerateNextOrderNumber
	{
		public static Int32 GetNextOrderNumber(AppDbContext _contect)
		{
			const Int32 START_NUMBER = 1000;

			Int32 intMaxOrderNumber;
			Int32 intNextOrderNumber;

			if (_contect.Orders.Count() == 0)
			{
				intMaxOrderNumber = START_NUMBER;
			}
			else
			{
				intMaxOrderNumber = _contect.Orders.Max(c => c.OrderNumber);
			}

			if (intMaxOrderNumber < START_NUMBER)
			{
				intMaxOrderNumber = START_NUMBER;
			}

			intNextOrderNumber = intMaxOrderNumber + 1;

			return intNextOrderNumber;
		}
	}
}

