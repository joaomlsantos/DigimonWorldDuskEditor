using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DigimonWorldDuskEditor.Models;
using DigimonWorldDuskEditor.Services;

namespace DigimonWorldDuskEditor.Forms
{
    public class EvolutionEditor : UserControl
    {
        private ListBox areaListBox;

        public EvolutionEditor(List<OffsetAddress> offsetAddresses)
        {
            InitializeComponents(offsetAddresses);
        }

        private void InitializeComponents(List<OffsetAddress> offsetAddresses)
        {
            this.Dock = DockStyle.Fill;

            areaListBox = new ListBox { Dock = DockStyle.Left, Width = 250 };
    
            foreach (var offsetAddress in offsetAddresses)
            {
                areaListBox.Items.Add(offsetAddress.AreaName);
            }
    
            Controls.Add(areaListBox);
        }

        public void LoadData(BinaryFileService binaryFileService)
        {
            // Load the data and enable controls
            areaListBox.Enabled = true;
        }

        public void SaveData(BinaryFileService binaryFileService)
        {
            // Implement the save logic for evolution data
        }
    }
}