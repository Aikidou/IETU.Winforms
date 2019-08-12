using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Forms;
using ReactiveUI;

namespace ImageEnhancingUtility.Train.Winforms
{
    public partial class TrainForm : Form, IViewFor<IETU>
    {
        public IETU ViewModel { get; set; }
        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (IETU)value;
        }

        int _test;
        public int test
        {
            get => _test;
            set
            {
                int x = hrSize_comboBox.Items.IndexOf(value);
                _test = x;
            }
        }

        List<int> _checkedDownscaleTypes;
        public List<int> CheckedDownscaleTypes
        {
            get => _checkedDownscaleTypes;
            set
            {
                foreach(int o in value)
                {
                    int i = lrDownscaleTypes_checkedListBox.Items.IndexOf((DownscaleType)o);
                    lrDownscaleTypes_checkedListBox.SetItemChecked(i, true);
                }
                _checkedDownscaleTypes = value;
            }
        }

        private delegate void SafeCallDelegate(string text);
        public string RichBoxText
        {
            get => "";
            set
            {
                WriteToLogsThreadSafe(value);
            }
        }

        private void WriteToLogsThreadSafe(string text)
        {
            if (log_richTextBox.InvokeRequired)
            {
                var d = new SafeCallDelegate(WriteToLogsThreadSafe);
                Invoke(d, new object[] { text });
            }
            else
                log_richTextBox.Text = text;
            //richTextBox1.AppendText($"\n[{DateTime.Now}] {text}", System.Drawing.Color.White);
        }

        DataGridViewCellStyle GridEnabled = new DataGridViewCellStyle();
        DataGridViewCellStyle GridDisabled = new DataGridViewCellStyle() { ForeColor = System.Drawing.Color.DarkGray };

        public TrainForm()
        {
            InitializeComponent();

            basicSrPath_button.Tag = basicSrPath_textBox;
            hrPath_button.Tag = hrPath_textBox;
            lrPath_button.Tag = lrPath_textBox;
            valHrPath_button.Tag = valHrPath_textBox;
            valLrPath_button.Tag = valLrPath_textBox;
            pretrainedPath_button.Tag = pretrainedPath_textBox;
            resumeStatePath_button.Tag = resumeStatePath_textBox;
            datasetFolderPath_button.Tag = datasetFolderPath_textBox;

            ViewModel = new IETU();

            this.Bind(ViewModel, vm => vm.DatasetFolderPath, v => v.datasetFolderPath_textBox.Text);
            this.Bind(ViewModel, vm => vm.IgnoreFewColorsTiles, v => v.ignoreFewColorsTiles_checkBox.Checked);
            this.Bind(ViewModel, vm => vm.ValidationTileNumber, v => v.validationTileNumber_numericUpDown.Value, x => x, y => decimal.ToInt32(y));

            this.Bind(ViewModel, vm => vm.TrainConfig.Logger.SaveCheckpointFreq, v => v.saveStep_numericUpDown.Value, x => x, y => decimal.ToInt32(y));
            this.Bind(ViewModel, vm => vm.TrainConfig.Logger.PrintFreq, v => v.printStep_numericUpDown.Value, x => x, y => decimal.ToInt32(y));
            this.Bind(ViewModel, vm => vm.TrainConfig.Train.ValFreq, v => v.valStep_numericUpDown.Value, x => x, y => decimal.ToInt32(y));

            //this.WhenAnyValue(x => x.valStep_numericUpDown.Value).BindTo(this, x => x.ViewModel.TrainConfig.Train).S, x => decimal.ToInt32(x));
            //this.WhenAnyValue(x => x.ViewModel.TrainConfig).Select(x => (decimal) x.Train.ValFreq).BindTo(this, x => x.test);

            this.Bind(ViewModel, vm => vm.TrainConfig.Path.Root, v => v.basicSrPath_textBox.Text);
            this.Bind(ViewModel, vm => vm.TrainConfig.Datasets.Train.DatarootHR, v => v.hrPath_textBox.Text);
            this.Bind(ViewModel, vm => vm.TrainConfig.Datasets.Train.DatarootLR, v => v.lrPath_textBox.Text);
            this.Bind(ViewModel, vm => vm.TrainConfig.Datasets.Val.DatarootHR, v => v.valHrPath_textBox.Text);
            this.Bind(ViewModel, vm => vm.TrainConfig.Datasets.Val.DatarootLR, v => v.valLrPath_textBox.Text);
            this.Bind(ViewModel, vm => vm.TrainConfig.Path.PretrainModelG, v => v.pretrainedPath_textBox.Text);
            this.Bind(ViewModel, vm => vm.TrainConfig.Path.ResumeState, v => v.resumeStatePath_textBox.Text);
            this.Bind(ViewModel, vm => vm.TrainConfig.Name, v => v.modelName_textBox.Text);

            modelScale_comboBox.DataSource = IETU.ScaleSizes;
            hrSize_comboBox.DataSource = IETU.HrSizes;

            this.Bind(ViewModel, vm => vm.TrainConfig.Scale, v => v.modelScale_comboBox.SelectedIndex, v => IETU.ScaleSizes.IndexOf(v), v => IETU.ScaleSizes[v]);
            this.Bind(ViewModel, vm => vm.TrainConfig.Datasets.Train.NWorkers, v => v.workers_numericUpDown.Value, x => x, y => decimal.ToInt32(y));
            this.Bind(ViewModel, vm => vm.TrainConfig.Datasets.Train.BatchSize, v => v.batchSize_numericUpDown.Value, x => x, y => decimal.ToInt32(y));            
            this.Bind(ViewModel, vm => vm.TrainConfig.Datasets.Train.HRSize, v => v.hrSize_comboBox.SelectedIndex, z => IETU.HrSizes.IndexOf(z), z => IETU.HrSizes[z]);

            this.Bind(ViewModel, vm => vm.TrainConfig.Datasets.Train.LrDownscale, v => v.lrDownscaleEnabled_checkBox.Checked);
            this.Bind(ViewModel, vm => vm.TrainConfig.Datasets.Train.LrNoise, v => v.lrNoiseEnabled_checkBox.Checked);
            this.Bind(ViewModel, vm => vm.TrainConfig.Datasets.Train.HrCrop, v => v.hrCropEnabled_checkBox.Checked);
            this.Bind(ViewModel, vm => vm.TrainConfig.Datasets.Train.HrRrot, v => v.hrRotationEnabled_checkBox.Checked);

            this.Bind(ViewModel, vm => vm.UseHrAsLr, v => v.useHrAsLr_checkBox.Checked);
            this.Bind(ViewModel, vm => vm.DisablePretrainedModel, v => v.disablePretrained_checkBox.Checked);
            this.Bind(ViewModel, vm => vm.DisableResumeState, v => v.disableResumeState_checkBox.Checked);

            this.Bind(ViewModel, vm => vm.Configs, v => v.configs_listBox.DataSource);
            configs_listBox.ValueMember = "Name";
            configs_listBox.DisplayMember = "Name";

            this.Bind(ViewModel, vm => vm.SaveConfigName, v => v.saveConfigName_textBox.Text);
            this.Bind(ViewModel, vm => vm.SelectedConfigName, v => v.configs_listBox.SelectedValue, x => x, x => x?.ToString());

            this.BindCommand(ViewModel, vm => vm.SaveConfigCommand, v => v.saveConfig_button);
            ViewModel.SaveConfigCommand.ThrownExceptions.Subscribe(exception => { });
            this.BindCommand(ViewModel, vm => vm.LoadConfigCommand, v => v.loadConfig_button);
            ViewModel.LoadConfigCommand.ThrownExceptions.Subscribe(exception => { throw exception; });
            this.BindCommand(ViewModel, vm => vm.DeleteConfigCommand, v => v.deleteConfig_button);
            ViewModel.DeleteConfigCommand.ThrownExceptions.Subscribe(exception => { });

            lrDownscaleTypes_checkedListBox.DataSource = Enum.GetValues(typeof(DownscaleType));
            lrDownscaleTypes_checkedListBox.Enabled = false;
            this.OneWayBind(ViewModel, vm => vm.TrainConfig.Datasets.Train.LrDownscaleTypes, v => v.CheckedDownscaleTypes, x => x.Cast<int>().ToList());


            lrNoise_dataGridView.DataSource = ViewModel.NoiseValues;
            lrNoise_dataGridView.Columns[0].ReadOnly = true;
            lrNoise_dataGridView.Columns[0].Width = 60;
            lrNoise_dataGridView.Columns[1].Width = 35;
            lrNoise_dataGridView.Columns[1].ReadOnly = false;
            lrNoise_dataGridView.Enabled = false;
            lrNoise_dataGridView.DefaultCellStyle = GridDisabled;

            this.OneWayBind(ViewModel, vm => vm.Core.Logs, v => v.RichBoxText);

        }

        private void lrDownscaleTypes_checkedListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            CheckedListBox checkedListBox = sender as CheckedListBox;

            var selectedItem = checkedListBox.SelectedItem;
            ViewModel.TrainConfig.Datasets.Train.LrDownscaleTypes = checkedListBox.CheckedItems.Cast<DownscaleType>().ToList(); //hack
            if (checkedListBox.CheckedItems.Contains(selectedItem))
                ViewModel.TrainConfig.Datasets.Train.LrDownscaleTypes.Remove((DownscaleType)checkedListBox.SelectedItem);
            else
                ViewModel.TrainConfig.Datasets.Train.LrDownscaleTypes.Add((DownscaleType)checkedListBox.SelectedItem);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            IETU test = ViewModel;            
        }


        private void selectFolderPath_button_Click(object sender, EventArgs e)
        {
            TextBox textBox = (sender as Button).Tag as TextBox;
            FolderSelectDialog.FolderSelectDialog folderSelectDialog = new FolderSelectDialog.FolderSelectDialog();
            folderSelectDialog.ShowDialog();
            if (folderSelectDialog.FileName != "")
                textBox.Text = folderSelectDialog.FileName;
        }

        private void selectFilePath_button_Click(object sender, EventArgs e)
        {
            TextBox textBox = (sender as Button).Tag as TextBox;       
            FileDialog fileSelectDialog = new OpenFileDialog();
            if (fileSelectDialog.ShowDialog() == DialogResult.OK)
                textBox.Text = fileSelectDialog.FileName;
        }

        private void configs_listBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(configs_listBox.SelectedValue != null)
                saveConfigName_textBox.Text = configs_listBox.SelectedValue.ToString().Replace(".json","");
        }

        private void useHrAsLr_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            hrCropEnabled_checkBox.Enabled = useHrAsLr_checkBox.Checked;
            hrRotationEnabled_checkBox.Enabled = useHrAsLr_checkBox.Checked;  
            lrPath_textBox.Enabled = !useHrAsLr_checkBox.Checked;
            valLrPath_textBox.Enabled = !useHrAsLr_checkBox.Checked;
        }

        private void lrNoiseEnabled_checkBox_CheckedChanged(object sender, EventArgs e)
        {            
            lrNoise_dataGridView.Enabled = lrNoiseEnabled_checkBox.Checked;
            lrNoise_dataGridView.DefaultCellStyle = lrNoiseEnabled_checkBox.Checked?GridEnabled: GridDisabled;
        }

        private void lrDownscaleEnabled_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            lrDownscaleTypes_checkedListBox.Enabled = lrDownscaleEnabled_checkBox.Checked;
        }

        private void disablePretrained_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            pretrainedPath_textBox.Enabled = !disablePretrained_checkBox.Checked;
        }

        private void disableResumeState_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            resumeStatePath_textBox.Enabled = !disableResumeState_checkBox.Checked;
        }

        private void prepareHr_button_Click(object sender, EventArgs e)
        {
            ViewModel.PrepareHR();
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            // set the current caret position to the end
            log_richTextBox.SelectionStart = log_richTextBox.Text.Length;
            // scroll it automatically
            log_richTextBox.ScrollToCaret();
        }

        private void startTrain_button_Click(object sender, EventArgs e)
        {
            ViewModel.StartTrain();
        }
    }

}
