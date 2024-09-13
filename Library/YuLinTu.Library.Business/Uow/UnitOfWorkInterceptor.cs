using Castle.DynamicProxy;
using System;

namespace YuLinTu.Library.Business
{
    public class UnitOfWorkInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            if (!UnitOfWorkHelper.IsUnitOfWorkMethod(invocation.Method, out var unitOfWorkAttribute))
            {
                invocation.Proceed();
                return;
            }

            //lock (this)
            //{
            try
            {
                Console.WriteLine("开启事务");
                Db.GetInstance().BeginTransaction();

                invocation.Proceed();

                Console.WriteLine("结束事务");
                Db.GetInstance().CommitTransaction();
            }
            catch (Exception)
            {
                Console.WriteLine("事务回滚");
                Db.GetInstance().RollbackTransaction();
                throw;
            }
            //}
        }
    }
}