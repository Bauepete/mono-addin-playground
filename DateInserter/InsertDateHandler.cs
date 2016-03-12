using MonoDevelop.Components.Commands;
using MonoDevelop.Core;
using MonoDevelop.Ide;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Projects;
using Mono.TextEditor;
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace DateInserter
{
    public class InsertDateHandler : CommandHandler
    {
        private const string LINES_COUNT_DOCUMENT_NAME = "Lines Count Statistics";

        protected override void Update(CommandInfo info)
        {
            object selectedItem = GetSelectedItem();

            if (selectedItem == null)
            {
                info.Enabled = false;
                return;
            }

            if (selectedItem is Project || selectedItem is Solution)
                info.Enabled = true;
            else
            {
                ProjectFile selectedFile = selectedItem as ProjectFile;
                info.Enabled = selectedFile != null && LinesCountWriter.IsCSharpFile(selectedFile);
            }
        }

        protected override void Run()
        {
            LinesCountWriter w = new LinesCountWriter(LINES_COUNT_DOCUMENT_NAME);
            object selectedItem = GetSelectedItem();
            if (selectedItem != null)
                w.WriteInfoOfSelectedItem(selectedItem);
        }

        static object GetSelectedItem()
        {
            return IdeApp.ProjectOperations.CurrentSelectedItem;
        }
    }
}

