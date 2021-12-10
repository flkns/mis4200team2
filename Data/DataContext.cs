using mis4200team2.Models;
using System;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Interception;
using System.Diagnostics;
using System.Data.Entity.SqlServer;
using System.Linq;
//using mis4200team2.Logging;
using System.Data.SqlClient;
using System.Reflection;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Configuration;

namespace mis4200team2.Data
{
    public class DataContext : DbContext
    {
        public DataContext() : base("name=DefaultConnection") { }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Kudos> KudosDB { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
      base.OnModelCreating(modelBuilder);
            //modelBuilder.Entity<Employee>().ToTable(nameof(Employee));
            //modelBuilder.Entity<Kudos>().ToTable(nameof(Kudos));

        //modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();  // note: this is all one line!
      }
    }


  //public class DataInterceptorLogging : DbCommandInterceptor
  //{
  //  private ILogger _logger = new Logger();
  //  private readonly Stopwatch _stopwatch = new Stopwatch();

  //  public override void ScalarExecuting(DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
  //  {
  //    base.ScalarExecuting(command, interceptionContext);
  //    _stopwatch.Restart();
  //  }

  //  public override void ScalarExecuted(DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
  //  {
  //    _stopwatch.Stop();
  //    if (interceptionContext.Exception != null)
  //    {
  //      _logger.Error(interceptionContext.Exception, "Error executing command: {0}", command.CommandText);
  //    }
  //    else
  //    {
  //      _logger.TraceApi("SQL Database", "DataInterceptor.ScalarExecuted", _stopwatch.Elapsed, "Command: {0}: ", command.CommandText);
  //    }
  //  }

  //  public override void NonQueryExecuting(DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
  //  {
  //    base.NonQueryExecuting(command, interceptionContext);
  //    _stopwatch.Restart();
  //  }

  //  public override void NonQueryExecuted(DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
  //  {
  //    _stopwatch.Stop();
  //    if (interceptionContext.Exception != null)
  //    {
  //      _logger.Error(interceptionContext.Exception, "Error executing command: {0}", command.CommandText);
  //    }
  //    else
  //    {
  //      _logger.TraceApi("SQL Database", "DataInterceptor.NonQueryExecuted", _stopwatch.Elapsed, "Command: {0}: ", command.CommandText);
  //    }
  //    base.NonQueryExecuted(command, interceptionContext);
  //  }

  //  public override void ReaderExecuting(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
  //  {
  //    base.ReaderExecuting(command, interceptionContext);
  //    _stopwatch.Restart();
  //  }
  //  public override void ReaderExecuted(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
  //  {
  //    _stopwatch.Stop();
  //    if (interceptionContext.Exception != null)
  //    {
  //      _logger.Error(interceptionContext.Exception, "Error executing command: {0}", command.CommandText);
  //    }
  //    else
  //    {
  //      _logger.TraceApi("SQL Database", "DataInterceptor.ReaderExecuted", _stopwatch.Elapsed, "Command: {0}: ", command.CommandText);
  //    }
  //    base.ReaderExecuted(command, interceptionContext);
  //  }*//
  //}

  //public class DataInterceptorTransientErrors : DbCommandInterceptor
  //{
  //  private int _counter = 0;
  //  private ILogger _logger = new Logger();

  //  public override void ReaderExecuting(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
  //  {
  //    bool throwTransientErrors = false;
  //    if (command.Parameters.Count > 0 && command.Parameters[0].Value.ToString() == "%Throw%")
  //    {
  //      throwTransientErrors = true;
  //      command.Parameters[0].Value = "%an%";
  //      command.Parameters[1].Value = "%an%";
  //    }

  //    if (throwTransientErrors && _counter < 4)
  //    {
  //      _logger.Info("Returning transient error for command: {0}", command.CommandText);
  //      _counter++;
  //      interceptionContext.Exception = CreateDummySqlException();
  //    }
  //  }

  //  private SqlException CreateDummySqlException()
  //  {
  //    // The instance of SQL Server you attempted to connect to does not support encryption
  //    var sqlErrorNumber = 20;

  //    var sqlErrorCtor = typeof(SqlError).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).Where(c => c.GetParameters().Count() == 7).Single();
  //    var sqlError = sqlErrorCtor.Invoke(new object[] { sqlErrorNumber, (byte)0, (byte)0, "", "", "", 1 });

  //    var errorCollection = Activator.CreateInstance(typeof(SqlErrorCollection), true);
  //    var addMethod = typeof(SqlErrorCollection).GetMethod("Add", BindingFlags.Instance | BindingFlags.NonPublic);
  //    addMethod.Invoke(errorCollection, new[] { sqlError });

  //    var sqlExceptionCtor = typeof(SqlException).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).Where(c => c.GetParameters().Count() == 4).Single();
  //    var sqlException = (SqlException)sqlExceptionCtor.Invoke(new object[] { "Dummy", errorCollection, null, Guid.NewGuid() });

  //    return sqlException;
  //  }
  //}
}
