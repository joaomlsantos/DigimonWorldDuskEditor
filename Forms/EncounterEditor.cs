using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DigimonWorldDuskEditor.Models;
using DigimonWorldDuskEditor.Services;

namespace DigimonWorldDuskEditor.Forms
{
    public class EncounterEditor : UserControl
    {
        private ListBox areaListBox;
        private Panel valuePanel;
        private Dictionary<ComboBox, long> valueComboBoxOffsets;
        public Dictionary<string, List<(byte[], long)>> preloadedValues;

        public EncounterEditor(List<OffsetAddress> offsetAddresses)
        {
            this.valueComboBoxOffsets = new Dictionary<ComboBox, long>();
            this.preloadedValues = new Dictionary<string, List<(byte[], long)>>();

            InitializeComponents(offsetAddresses);
        }
        
        public void PreloadValues(BinaryFileService binaryFileService, List<OffsetAddress> offsetAddresses)
        {
            var interval = 0x18;
            foreach (var offsetAddress in offsetAddresses)
            {
                var offset = offsetAddress.Offset;
                var values = binaryFileService.ReadValuesAtIntervals(offset + 0x10, interval, 0x00, 0xFF);
                preloadedValues[offsetAddress.AreaName] = values;
            }
        }

        public void InitializeComponents(List<OffsetAddress> offsetAddresses)
        {
            this.Dock = DockStyle.Fill;

            areaListBox = new ListBox { Dock = DockStyle.Left, Width = 250, Enabled = false  };
            valuePanel = new Panel { Dock = DockStyle.Fill, Enabled = false  };

            areaListBox.SelectedIndexChanged += AreaListBox_SelectedIndexChanged;
    
            foreach (var offsetAddress in offsetAddresses)
            {
                areaListBox.Items.Add(offsetAddress.AreaName);
            }
    
            Controls.Add(valuePanel);
            Controls.Add(areaListBox);
        }

        public void LoadData(BinaryFileService binaryFileService, List<OffsetAddress> offsetAddresses)
        {
            preloadedValues.Clear();
            PreloadValues(binaryFileService, offsetAddresses);
            // Load the data and enable controls
            areaListBox.Enabled = true;
            valuePanel.Enabled = true;

            // Populate preloaded values
            foreach (var offsetAddress in offsetAddresses)
            {
                var offset = offsetAddress.Offset;
                var values = binaryFileService.ReadValuesAtIntervals(offset + 0x10, 0x18, 0x00, 0xFF);
                this.preloadedValues[offsetAddress.AreaName] = values;
            }
        }

        private void AreaListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedArea = areaListBox.SelectedItem.ToString();
            if (preloadedValues.ContainsKey(selectedArea))
            {
                LoadValues(preloadedValues[selectedArea]);
            }
        }

        private void LoadValues(List<(byte[], long)> values)
        {
            valuePanel.SuspendLayout();
            valuePanel.Controls.Clear();
            valueComboBoxOffsets.Clear();

            var comboBoxes = new List<ComboBox>();
            int yPos = 10;

            foreach (var (value, offset) in values)
            {
                var hexValue = Convert.ToHexString(value);
                var mapping = ((MainForm)ParentForm).valueMappings.FirstOrDefault(v => Convert.ToHexString(v.HexValue) == hexValue);
                ComboBox comboBox = new ComboBox { Width = 200 };

                if (mapping != null)
                {
                    comboBox.Items.Add(mapping.ValueName);
                    comboBox.SelectedItem = mapping.ValueName;
                }
                else
                {
                    comboBox.Items.Add(hexValue);
                    comboBox.SelectedItem = hexValue;
                }

                comboBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                comboBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
                comboBox.AutoCompleteCustomSource.AddRange(((MainForm)ParentForm).valueMappings.Select(vm => vm.ValueName).ToArray());

                valueComboBoxOffsets[comboBox] = offset;
                comboBoxes.Add(comboBox);
            }

            int comboBoxIter = 0;
            foreach (var comboBox in comboBoxes)
            {
                if (comboBoxIter != 0 && comboBoxIter % 2 == 0)
                    yPos += 40;

                comboBox.Top = yPos;
                comboBox.Left = 30 + 240 * (comboBoxIter % 2);
                valuePanel.Controls.Add(comboBox);
                comboBoxIter += 1;
            }

            valuePanel.ResumeLayout();
        }

        public void SaveData(BinaryFileService binaryFileService)
        {
            var selectedArea = areaListBox.SelectedItem.ToString();
            var newPreloadedValues = new List<(byte[], long)>();

            foreach (var comboBox in valueComboBoxOffsets.Keys)
            {
                var offset = valueComboBoxOffsets[comboBox];
                var selectedValue = comboBox.SelectedItem.ToString();
                var newData = ((MainForm)ParentForm).valueMappings.FirstOrDefault(vm => vm.ValueName == selectedValue)?.HexValue;

                if (newData != null)
                {
                    binaryFileService.WriteBytes(offset, newData);
                }
                else
                {
                    MessageBox.Show("Invalid value selected.");
                }
                newPreloadedValues.Add((newData, offset));
            }

            preloadedValues[selectedArea] = newPreloadedValues;
            MessageBox.Show("Data saved successfully.");
        }
    }
}