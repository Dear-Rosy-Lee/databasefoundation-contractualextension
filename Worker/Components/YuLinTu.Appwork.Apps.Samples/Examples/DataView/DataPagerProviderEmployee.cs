using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using YuLinTu;
using YuLinTu.Data;
using YuLinTu.Data.Dynamic;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Appwork.Apps.Samples
{
    public class DataPagerProviderEmployee : IDataPagerProvider
    {
        #region Properties

        #endregion

        #region Fields

        #endregion

        #region Ctor

        public DataPagerProviderEmployee()
        {
        }

        #endregion

        #region Methods

        public List<object> Paging(int pageIndex, int pageSize, string orderPropertyName, eOrder order)
        {
            var orders = Xceed.Wpf.Samples.SampleData.SampleDataProvider.GetOrders().Select(c =>
            {
                var employee = c.ConvertTo<EmployeeItem>();
                employee.Name = $"{c.Employee.FirstName} {c.Employee.LastName}";
                employee.Photo = c.Employee.SmallPhoto;
                return employee;
            });

            var list = orders.AsQueryable().
                OrderBy(string.Format("{0} {1}", orderPropertyName, order == eOrder.Ascending ? "asc" : "desc")).
                Skip(pageIndex * pageSize).
                Take(pageSize).
                ToList();

            return list.Cast<object>().ToList();
        }

        public int Count()
        {
            return Xceed.Wpf.Samples.SampleData.SampleDataProvider.GetOrders().Count;
        }

        public PropertyMetadata[] GetAttributeProperties()
        {
            return GetProperties();
        }

        public PropertyMetadata[] GetProperties()
        {
            return new PropertyMetadata[0];
        }

        public string GetDefaultOrderPropertyName()
        {
            return "OrderID";
        }

        #endregion

    }
}
