﻿using OctaneMyItemsSyncService.Services;
using System.Threading.Tasks;
using System.Windows.Forms;
using Office = Microsoft.Office.Core;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace OctaneMyItems
{
  public partial class ThisAddIn
  {
    private static Configuration m_configuration;
    private void ThisAddIn_Startup(object sender, System.EventArgs e)
    {
      m_configuration = new Configuration(this.Application);
    }

    private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
    {
      // Note: Outlook no longer raises this event. If you have code that 
      //    must run when Outlook shuts down, see http://go.microsoft.com/fwlink/?LinkId=506785
    }
    public static Configuration Configuration
    {
      get
      {
        return m_configuration;
      }
    }

    public async static Task<bool> GetConfiguration()
    {
      if (!m_configuration.IsInitialized)
        await m_configuration.GetConfiguration();

      return m_configuration.IsInitialized;
    }

    public static void ShowConfiguration()
    {
      m_configuration.ShowConfiguration();
    }

    public static async Task SyncOne()
    {
      try
      {
        await OctaneTask.SyncOne();
      }
      catch (System.Exception ex)
      {
        MessageBox.Show(ex.ToString());
      }
    }
    public static async Task SyncAll()
    {
      try
      {
        if (await GetConfiguration())
        {
          var octaneService = m_configuration.OctaneService;
          OctaneTask.AddOctaneCategories();

          // sync backlog item
          var myBacklogs = await octaneService.GetMyBacklogs();
          await OctaneTask.ClearOldTaskItem(myBacklogs.data, Constants.CategoryOctaneBacklog);
          foreach (OctaneMyItemsSyncService.Models.Backlog backlog in myBacklogs.data)
          {
            OctaneTask.CreateTask(backlog).ConfigureAwait(false);
          }

          // sync run
          var runs = await octaneService.GetMyRuns();
          await OctaneTask.ClearOldTaskItem(runs.data, Constants.CategoryOctaneRun);
          foreach (OctaneMyItemsSyncService.Models.Run run in runs.data)
          {
            OctaneTask.CreateTask(run).ConfigureAwait(false);
          }

          // sync test
          var tests = await octaneService.GetMyTests();
          await OctaneTask.ClearOldTaskItem(tests.data, Constants.CategoryOctaneTest);
          foreach (OctaneMyItemsSyncService.Models.Test test in tests.data)
          {
            OctaneTask.CreateTask(test).ConfigureAwait(false);
          }
          UpdateCurrentSelection();
        }
      }
      catch (System.Exception ex)
      {
        MessageBox.Show(ex.ToString());
      }
    }

    public static async Task SyncBacklog()
    {
      try
      {
        if (await GetConfiguration())
        {
          OctaneTask.AddOctaneCategories();

          var myBacklogs = await m_configuration.OctaneService.GetMyBacklogs();
          await OctaneTask.ClearOldTaskItem(myBacklogs.data, Constants.CategoryOctaneBacklog);
          foreach (OctaneMyItemsSyncService.Models.Backlog backlog in myBacklogs.data)
            OctaneTask.CreateTask(backlog).ConfigureAwait(false);
          UpdateCurrentSelection();
        }
      }
      catch (System.Exception ex)
      {
        MessageBox.Show(ex.ToString());
      }
    }
    public static async Task SyncTest()
    {
      try
      {
        if (await GetConfiguration())
        {
          OctaneTask.AddOctaneCategories();

          var tests = await m_configuration.OctaneService.GetMyTests();
          await OctaneTask.ClearOldTaskItem(tests.data, Constants.CategoryOctaneTest);
          foreach (OctaneMyItemsSyncService.Models.Test test in tests.data)
            OctaneTask.CreateTask(test).ConfigureAwait(false);
          UpdateCurrentSelection();
        }
      }
      catch (System.Exception ex)
      {
        MessageBox.Show(ex.ToString());
      }
    }
    public static async Task SyncRun()
    {
      try
      {
        if (await GetConfiguration())
        {
          OctaneTask.AddOctaneCategories();

          var runs = await m_configuration.OctaneService.GetMyRuns();
          await OctaneTask.ClearOldTaskItem(runs.data, Constants.CategoryOctaneRun);
          foreach (OctaneMyItemsSyncService.Models.Run run in runs.data)
            OctaneTask.CreateTask(run).ConfigureAwait(false);
          UpdateCurrentSelection();
        }
      }
      catch (System.Exception ex)
      {
        MessageBox.Show(ex.ToString());
      }
    }

    private static void UpdateCurrentSelection()
    {
      var explorer = Globals.ThisAddIn.Application.Application.ActiveExplorer();
      try
      {
        if (explorer.Selection[1] is Outlook.TaskItem)
          ((Outlook.TaskItem)explorer.Selection[1]).Close(Outlook.OlInspectorClose.olSave);
      }
      catch (System.Exception)
      {
      }
    }

    protected override Microsoft.Office.Core.IRibbonExtensibility CreateRibbonExtensibilityObject()
    {
      return new Ribbon2();
    }
    #region VSTO generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InternalStartup()
    {
      this.Startup += new System.EventHandler(ThisAddIn_Startup);
      this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
    }

    #endregion
  }
}