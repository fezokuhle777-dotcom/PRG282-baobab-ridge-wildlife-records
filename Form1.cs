using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BaobabRidge
{
    public partial class Form1 : Form
    {
        
        public Form1()
        {
            InitializeComponent();

            
            SetupAnimalGrid();
            LoadAnimals();
            AnimalsView();

           
        }

        MessageBox.Show(path);
        private void AnimalsView()
        {
            DataTable table = new DataTable();
            table.Columns.Add("Animal ID");
            table.Columns.Add("Name");

            dgvAnimals.DataSource = table;
        }


        private readonly string filePath =
        Path.Combine(AppContext.BaseDirectory, "animals.txt");

        // Configures the grid columns and prevents direct editing.
        private void SetupAnimalGrid()
        {
            dgvAnimals.Columns.Clear();
            dgvAnimals.AutoGenerateColumns = false;

            dgvAnimals.ReadOnly = true;
            dgvAnimals.AllowUserToAddRows = false;
            dgvAnimals.AllowUserToDeleteRows = false;
            dgvAnimals.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;
            dgvAnimals.MultiSelect = false;
            dgvAnimals.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            AddGridColumn("AnimalID", "Animal ID");
            AddGridColumn("Name", "Name");
            AddGridColumn("Species", "Species");
            AddGridColumn("Age", "Age");
            AddGridColumn("RecoveryScore", "Recovery Score");

            AnimalsView();
        }

        // Adds a read-only column with the correct heading.
        private void AddGridColumn(string property, string heading)
        {
            dgvAnimals.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = property,
                Name = property,
                HeaderText = heading,
                ReadOnly = true
            });
        }

        // 5. File reading method
        private List<Animals> ReadAnimals(out int skippedLines)
        {
            List<Animals> animals = new List<Animals>();
            skippedLines = 0;

            if (!File.Exists(filePath))
            {
                using (File.Create(filePath)) { }
                return animals;
            }

            string[] lines = File.ReadAllLines(
                filePath, Encoding.UTF8);

            foreach (string line in lines)
            {
                string[] fields = line.Split('|');

                if (fields.Length != 7)
                {
                    skippedLines++;
                    continue;
                }

                string id = fields[0].Trim().ToUpperInvariant();

                bool validID = Regex.IsMatch(
                    id, @"^WR-[0-9]{4}$");

                bool validAge = int.TryParse(
                    fields[3], out int age);

                bool validScore = int.TryParse(
                    fields[4], out int score);

                if (!validID || !validAge || !validScore ||
                    age < 0 || age > 100 ||
                    score < 0 || score > 100)
                {
                    skippedLines++;
                    continue;
                }

                Animals animal = new Animals
                {
                    AnimalID = id,
                    Name = fields[1],
                    Species = fields[2],
                    Age = age,
                    RecoveryScore = score
                };

                animals.Add(animal);
            }

            return animals;
        }

        // 6. Grid loading method
        public void LoadAnimals()
        {
            try
            {
                int skippedLines;

                List<Animals> animals =
                    ReadAnimals(out skippedLines);

                // Clear the old data before displaying new records.
                dgvAnimals.DataSource = null;
                dgvAnimals.DataSource = animals;

                dgvAnimals.ClearSelection();

                if (skippedLines > 0)
                {
                    MessageBox.Show(
                        skippedLines +
                        " invalid record(s) were skipped.",
                        "File Loading",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show(
                    "Permission denied when accessing animals.txt.",
                    "File Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            catch (IOException)
            {
                MessageBox.Show(
                    "Unable to access animals.txt. " +
                    "The file may be in use.",
                    "File Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
       
        // Deletes the selected animal from the file.
        public void DeleteAnimal(string animalID, string animalName)
        {
            DialogResult answer = MessageBox.Show(
                "Delete " + animalID + " (" + animalName +
                ")? This cannot be undone.",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (answer != DialogResult.Yes) return;

            try
            {
                List<string> lines = File.ReadAllLines(filePath, Encoding.UTF8).ToList();

                int removed = lines.RemoveAll(line =>
                    line.Split('|')[0].Trim().Equals(
                        animalID, StringComparison.OrdinalIgnoreCase));

                if (removed == 0)
                {
                    MessageBox.Show("Animal not found.");
                    LoadAnimals();
                    return;
                }

                File.WriteAllLines(filePath, lines, Encoding.UTF8);

                LoadAnimals();

                MessageBox.Show("Animal deleted successfully.");
            }
            catch (IOException)
            {
                MessageBox.Show("Animals.txt could not be read or saved. The file may be open in another program.");
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Permission denied.");
            }
        }

        // Refreshes the animal grid.
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadAnimals();
        }

        // Deletes the selected animal after confirmation.
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvAnimals.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an animal first.");
                return;
            }

            string id = dgvAnimals.SelectedRows[0]
                .Cells["AnimalID"].Value.ToString();

            string name = dgvAnimals.SelectedRows[0]
                .Cells["Name"].Value.ToString();

            DeleteAnimal(id, name);
        }

    }
}


namespace BaobabRidge
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.grpAnimalDetails = new System.Windows.Forms.GroupBox();
            this.txtAge = new System.Windows.Forms.TextBox();
            this.txtRecoveryScore = new System.Windows.Forms.TextBox();
            this.txtSpecies = new System.Windows.Forms.TextBox();
            this.txtName = new System.Windows.Forms.TextBox();
            this.txtAnimalId = new System.Windows.Forms.TextBox();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnSaveChanges = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.lblRecoveryScore = new System.Windows.Forms.Label();
            this.lblAge = new System.Windows.Forms.Label();
            this.lblSpecies = new System.Windows.Forms.Label();
            this.lblName = new System.Windows.Forms.Label();
            this.lblAnimalId = new System.Windows.Forms.Label();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.grpFindAnimal = new System.Windows.Forms.GroupBox();
            this.txtSearchId = new System.Windows.Forms.TextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.lblSearchId = new System.Windows.Forms.Label();
            this.grpSummaryReport = new System.Windows.Forms.GroupBox();
            this.btnGenerateSummary = new System.Windows.Forms.Button();
            this.lblCritical = new System.Windows.Forms.Label();
            this.lblReleaseReady = new System.Windows.Forms.Label();
            this.lblRecovering = new System.Windows.Forms.Label();
            this.lblStable = new System.Windows.Forms.Label();
            this.lblSerious = new System.Windows.Forms.Label();
            this.lblAverageScore = new System.Windows.Forms.Label();
            this.lblAverageAge = new System.Windows.Forms.Label();
            this.lblTotalAnimals = new System.Windows.Forms.Label();
            this.dgvAnimals = new System.Windows.Forms.DataGridView();
            this.btnDelete = new System.Windows.Forms.Button();
            this.lblTotalAnimalsCaption = new System.Windows.Forms.Label();
            this.lblAverageAgeCaption = new System.Windows.Forms.Label();
            this.lblAverageScoreCaption = new System.Windows.Forms.Label();
            this.lblReleaseReadyCaption = new System.Windows.Forms.Label();
            this.lblRecoveringCaption = new System.Windows.Forms.Label();
            this.lblStableCaption = new System.Windows.Forms.Label();
            this.lblSeriousCaption = new System.Windows.Forms.Label();
            this.lblCriticalCaption = new System.Windows.Forms.Label();
            this.grpAnimalDetails.SuspendLayout();
            this.grpFindAnimal.SuspendLayout();
            this.grpSummaryReport.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAnimals)).BeginInit();
            this.SuspendLayout();
            // 
            // grpAnimalDetails
            // 
            this.grpAnimalDetails.Controls.Add(this.txtAge);
            this.grpAnimalDetails.Controls.Add(this.txtRecoveryScore);
            this.grpAnimalDetails.Controls.Add(this.txtSpecies);
            this.grpAnimalDetails.Controls.Add(this.txtName);
            this.grpAnimalDetails.Controls.Add(this.txtAnimalId);
            this.grpAnimalDetails.Controls.Add(this.btnClear);
            this.grpAnimalDetails.Controls.Add(this.btnSaveChanges);
            this.grpAnimalDetails.Controls.Add(this.btnAdd);
            this.grpAnimalDetails.Controls.Add(this.lblRecoveryScore);
            this.grpAnimalDetails.Controls.Add(this.lblAge);
            this.grpAnimalDetails.Controls.Add(this.lblSpecies);
            this.grpAnimalDetails.Controls.Add(this.lblName);
            this.grpAnimalDetails.Controls.Add(this.lblAnimalId);
            this.grpAnimalDetails.Location = new System.Drawing.Point(14, 14);
            this.grpAnimalDetails.Margin = new System.Windows.Forms.Padding(4);
            this.grpAnimalDetails.Name = "grpAnimalDetails";
            this.grpAnimalDetails.Padding = new System.Windows.Forms.Padding(4);
            this.grpAnimalDetails.Size = new System.Drawing.Size(700, 313);
            this.grpAnimalDetails.TabIndex = 0;
            this.grpAnimalDetails.TabStop = false;
            this.grpAnimalDetails.Text = "Animal Details";
            // 
            // txtAge
            // 
            this.txtAge.Location = new System.Drawing.Point(213, 139);
            this.txtAge.Name = "txtAge";
            this.txtAge.Size = new System.Drawing.Size(229, 24);
            this.txtAge.TabIndex = 12;
            // 
            // txtRecoveryScore
            // 
            this.txtRecoveryScore.Location = new System.Drawing.Point(213, 181);
            this.txtRecoveryScore.Name = "txtRecoveryScore";
            this.txtRecoveryScore.Size = new System.Drawing.Size(229, 24);
            this.txtRecoveryScore.TabIndex = 11;
            // 
            // txtSpecies
            // 
            this.txtSpecies.Location = new System.Drawing.Point(213, 108);
            this.txtSpecies.Name = "txtSpecies";
            this.txtSpecies.Size = new System.Drawing.Size(229, 24);
            this.txtSpecies.TabIndex = 10;
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(213, 70);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(229, 24);
            this.txtName.TabIndex = 9;
            // 
            // txtAnimalId
            // 
            this.txtAnimalId.Location = new System.Drawing.Point(213, 32);
            this.txtAnimalId.Name = "txtAnimalId";
            this.txtAnimalId.Size = new System.Drawing.Size(229, 24);
            this.txtAnimalId.TabIndex = 8;
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(488, 229);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(176, 48);
            this.btnClear.TabIndex = 7;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            // 
            // btnSaveChanges
            // 
            this.btnSaveChanges.Enabled = false;
            this.btnSaveChanges.Location = new System.Drawing.Point(260, 229);
            this.btnSaveChanges.Name = "btnSaveChanges";
            this.btnSaveChanges.Size = new System.Drawing.Size(176, 48);
            this.btnSaveChanges.TabIndex = 6;
            this.btnSaveChanges.Text = "Save Changes";
            this.btnSaveChanges.UseVisualStyleBackColor = true;
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(18, 229);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(176, 48);
            this.btnAdd.TabIndex = 5;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            // 
            // lblRecoveryScore
            // 
            this.lblRecoveryScore.AutoSize = true;
            this.lblRecoveryScore.Location = new System.Drawing.Point(47, 184);
            this.lblRecoveryScore.Name = "lblRecoveryScore";
            this.lblRecoveryScore.Size = new System.Drawing.Size(119, 18);
            this.lblRecoveryScore.TabIndex = 4;
            this.lblRecoveryScore.Text = "Recovery Score:";
            // 
            // lblAge
            // 
            this.lblAge.AutoSize = true;
            this.lblAge.Location = new System.Drawing.Point(47, 142);
            this.lblAge.Name = "lblAge";
            this.lblAge.Size = new System.Drawing.Size(37, 18);
            this.lblAge.TabIndex = 3;
            this.lblAge.Text = "Age:";
            // 
            // lblSpecies
            // 
            this.lblSpecies.AutoSize = true;
            this.lblSpecies.Location = new System.Drawing.Point(47, 111);
            this.lblSpecies.Name = "lblSpecies";
            this.lblSpecies.Size = new System.Drawing.Size(65, 18);
            this.lblSpecies.TabIndex = 2;
            this.lblSpecies.Text = "Species:";
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(47, 73);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(52, 18);
            this.lblName.TabIndex = 1;
            this.lblName.Text = "Name:";
            // 
            // lblAnimalId
            // 
            this.lblAnimalId.AutoSize = true;
            this.lblAnimalId.Location = new System.Drawing.Point(47, 35);
            this.lblAnimalId.Name = "lblAnimalId";
            this.lblAnimalId.Size = new System.Drawing.Size(74, 18);
            this.lblAnimalId.TabIndex = 0;
            this.lblAnimalId.Text = "Animal ID:";
            // 
            // grpFindAnimal
            // 
            this.grpFindAnimal.Controls.Add(this.txtSearchId);
            this.grpFindAnimal.Controls.Add(this.btnSearch);
            this.grpFindAnimal.Controls.Add(this.lblSearchId);
            this.grpFindAnimal.Location = new System.Drawing.Point(735, 24);
            this.grpFindAnimal.Margin = new System.Windows.Forms.Padding(4);
            this.grpFindAnimal.Name = "grpFindAnimal";
            this.grpFindAnimal.Padding = new System.Windows.Forms.Padding(4);
            this.grpFindAnimal.Size = new System.Drawing.Size(606, 116);
            this.grpFindAnimal.TabIndex = 1;
            this.grpFindAnimal.TabStop = false;
            this.grpFindAnimal.Text = "Find Animal";
            // 
            // txtSearchId
            // 
            this.txtSearchId.Location = new System.Drawing.Point(120, 48);
            this.txtSearchId.Name = "txtSearchId";
            this.txtSearchId.Size = new System.Drawing.Size(229, 24);
            this.txtSearchId.TabIndex = 13;
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(401, 33);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(198, 45);
            this.btnSearch.TabIndex = 7;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            // 
            // lblSearchId
            // 
            this.lblSearchId.AutoSize = true;
            this.lblSearchId.Location = new System.Drawing.Point(25, 48);
            this.lblSearchId.Name = "lblSearchId";
            this.lblSearchId.Size = new System.Drawing.Size(74, 18);
            this.lblSearchId.TabIndex = 5;
            this.lblSearchId.Text = "Animal ID:";
            // 
            // grpSummaryReport
            // 
            this.grpSummaryReport.Controls.Add(this.lblCriticalCaption);
            this.grpSummaryReport.Controls.Add(this.lblSeriousCaption);
            this.grpSummaryReport.Controls.Add(this.lblStableCaption);
            this.grpSummaryReport.Controls.Add(this.lblRecoveringCaption);
            this.grpSummaryReport.Controls.Add(this.lblReleaseReadyCaption);
            this.grpSummaryReport.Controls.Add(this.lblAverageScoreCaption);
            this.grpSummaryReport.Controls.Add(this.lblAverageAgeCaption);
            this.grpSummaryReport.Controls.Add(this.lblTotalAnimalsCaption);
            this.grpSummaryReport.Controls.Add(this.btnGenerateSummary);
            this.grpSummaryReport.Controls.Add(this.lblCritical);
            this.grpSummaryReport.Controls.Add(this.lblReleaseReady);
            this.grpSummaryReport.Controls.Add(this.lblRecovering);
            this.grpSummaryReport.Controls.Add(this.lblStable);
            this.grpSummaryReport.Controls.Add(this.lblSerious);
            this.grpSummaryReport.Controls.Add(this.lblAverageScore);
            this.grpSummaryReport.Controls.Add(this.lblAverageAge);
            this.grpSummaryReport.Controls.Add(this.lblTotalAnimals);
            this.grpSummaryReport.Location = new System.Drawing.Point(735, 153);
            this.grpSummaryReport.Margin = new System.Windows.Forms.Padding(4);
            this.grpSummaryReport.Name = "grpSummaryReport";
            this.grpSummaryReport.Padding = new System.Windows.Forms.Padding(4);
            this.grpSummaryReport.Size = new System.Drawing.Size(606, 174);
            this.grpSummaryReport.TabIndex = 1;
            this.grpSummaryReport.TabStop = false;
            this.grpSummaryReport.Text = "Summary Report";
            // 
            // btnGenerateSummary
            // 
            this.btnGenerateSummary.Location = new System.Drawing.Point(17, 24);
            this.btnGenerateSummary.Name = "btnGenerateSummary";
            this.btnGenerateSummary.Size = new System.Drawing.Size(240, 42);
            this.btnGenerateSummary.TabIndex = 8;
            this.btnGenerateSummary.Text = "Generate Summary";
            this.btnGenerateSummary.UseVisualStyleBackColor = true;
            // 
            // lblCritical
            // 
            this.lblCritical.AutoSize = true;
            this.lblCritical.Location = new System.Drawing.Point(317, 21);
            this.lblCritical.Name = "lblCritical";
            this.lblCritical.Size = new System.Drawing.Size(57, 18);
            this.lblCritical.TabIndex = 10;
            this.lblCritical.Text = "Critical:";
            // 
            // lblReleaseReady
            // 
            this.lblReleaseReady.AutoSize = true;
            this.lblReleaseReady.Location = new System.Drawing.Point(317, 135);
            this.lblReleaseReady.Name = "lblReleaseReady";
            this.lblReleaseReady.Size = new System.Drawing.Size(107, 18);
            this.lblReleaseReady.TabIndex = 9;
            this.lblReleaseReady.Text = "Release-ready:";
            // 
            // lblRecovering
            // 
            this.lblRecovering.AutoSize = true;
            this.lblRecovering.Location = new System.Drawing.Point(317, 105);
            this.lblRecovering.Name = "lblRecovering";
            this.lblRecovering.Size = new System.Drawing.Size(83, 18);
            this.lblRecovering.TabIndex = 8;
            this.lblRecovering.Text = "Recovering";
            // 
            // lblStable
            // 
            this.lblStable.AutoSize = true;
            this.lblStable.Location = new System.Drawing.Point(317, 75);
            this.lblStable.Name = "lblStable";
            this.lblStable.Size = new System.Drawing.Size(53, 18);
            this.lblStable.TabIndex = 7;
            this.lblStable.Text = "Stable:";
            // 
            // lblSerious
            // 
            this.lblSerious.AutoSize = true;
            this.lblSerious.Location = new System.Drawing.Point(317, 48);
            this.lblSerious.Name = "lblSerious";
            this.lblSerious.Size = new System.Drawing.Size(63, 18);
            this.lblSerious.TabIndex = 6;
            this.lblSerious.Text = "Serious:";
            // 
            // lblAverageScore
            // 
            this.lblAverageScore.AutoSize = true;
            this.lblAverageScore.Location = new System.Drawing.Point(25, 135);
            this.lblAverageScore.Name = "lblAverageScore";
            this.lblAverageScore.Size = new System.Drawing.Size(109, 18);
            this.lblAverageScore.TabIndex = 5;
            this.lblAverageScore.Text = "Average Score:";
            // 
            // lblAverageAge
            // 
            this.lblAverageAge.AutoSize = true;
            this.lblAverageAge.Location = new System.Drawing.Point(25, 105);
            this.lblAverageAge.Name = "lblAverageAge";
            this.lblAverageAge.Size = new System.Drawing.Size(94, 18);
            this.lblAverageAge.TabIndex = 4;
            this.lblAverageAge.Text = "Average Age:";
            // 
            // lblTotalAnimals
            // 
            this.lblTotalAnimals.AutoSize = true;
            this.lblTotalAnimals.Location = new System.Drawing.Point(25, 75);
            this.lblTotalAnimals.Name = "lblTotalAnimals";
            this.lblTotalAnimals.Size = new System.Drawing.Size(101, 18);
            this.lblTotalAnimals.TabIndex = 3;
            this.lblTotalAnimals.Text = "Total Animals:";
            // 
            // dgvAnimals
            // 
            this.dgvAnimals.AllowUserToAddRows = false;
            this.dgvAnimals.AllowUserToDeleteRows = false;
            this.dgvAnimals.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvAnimals.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAnimals.Location = new System.Drawing.Point(14, 347);
            this.dgvAnimals.MultiSelect = false;
            this.dgvAnimals.Name = "dgvAnimals";
            this.dgvAnimals.ReadOnly = true;
            this.dgvAnimals.RowHeadersWidth = 51;
            this.dgvAnimals.RowTemplate.Height = 24;
            this.dgvAnimals.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvAnimals.Size = new System.Drawing.Size(1327, 252);
            this.dgvAnimals.TabIndex = 2;
            // 
            // btnDelete
            // 
            this.btnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDelete.Location = new System.Drawing.Point(14, 613);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(176, 48);
            this.btnDelete.TabIndex = 6;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // lblTotalAnimalsCaption
            // 
            this.lblTotalAnimalsCaption.AutoSize = true;
            this.lblTotalAnimalsCaption.Location = new System.Drawing.Point(140, 69);
            this.lblTotalAnimalsCaption.Name = "lblTotalAnimalsCaption";
            this.lblTotalAnimalsCaption.Size = new System.Drawing.Size(16, 18);
            this.lblTotalAnimalsCaption.TabIndex = 11;
            this.lblTotalAnimalsCaption.Text = "0";
            // 
            // lblAverageAgeCaption
            // 
            this.lblAverageAgeCaption.AutoSize = true;
            this.lblAverageAgeCaption.Location = new System.Drawing.Point(140, 105);
            this.lblAverageAgeCaption.Name = "lblAverageAgeCaption";
            this.lblAverageAgeCaption.Size = new System.Drawing.Size(16, 18);
            this.lblAverageAgeCaption.TabIndex = 12;
            this.lblAverageAgeCaption.Text = "0";
            // 
            // lblAverageScoreCaption
            // 
            this.lblAverageScoreCaption.AutoSize = true;
            this.lblAverageScoreCaption.Location = new System.Drawing.Point(140, 135);
            this.lblAverageScoreCaption.Name = "lblAverageScoreCaption";
            this.lblAverageScoreCaption.Size = new System.Drawing.Size(16, 18);
            this.lblAverageScoreCaption.TabIndex = 13;
            this.lblAverageScoreCaption.Text = "0";
            // 
            // lblReleaseReadyCaption
            // 
            this.lblReleaseReadyCaption.AutoSize = true;
            this.lblReleaseReadyCaption.Location = new System.Drawing.Point(430, 135);
            this.lblReleaseReadyCaption.Name = "lblReleaseReadyCaption";
            this.lblReleaseReadyCaption.Size = new System.Drawing.Size(16, 18);
            this.lblReleaseReadyCaption.TabIndex = 14;
            this.lblReleaseReadyCaption.Text = "0";
            // 
            // lblRecoveringCaption
            // 
            this.lblRecoveringCaption.AutoSize = true;
            this.lblRecoveringCaption.Location = new System.Drawing.Point(430, 105);
            this.lblRecoveringCaption.Name = "lblRecoveringCaption";
            this.lblRecoveringCaption.Size = new System.Drawing.Size(16, 18);
            this.lblRecoveringCaption.TabIndex = 15;
            this.lblRecoveringCaption.Text = "0";
            // 
            // lblStableCaption
            // 
            this.lblStableCaption.AutoSize = true;
            this.lblStableCaption.Location = new System.Drawing.Point(430, 75);
            this.lblStableCaption.Name = "lblStableCaption";
            this.lblStableCaption.Size = new System.Drawing.Size(16, 18);
            this.lblStableCaption.TabIndex = 16;
            this.lblStableCaption.Text = "0";
            // 
            // lblSeriousCaption
            // 
            this.lblSeriousCaption.AutoSize = true;
            this.lblSeriousCaption.Location = new System.Drawing.Point(430, 48);
            this.lblSeriousCaption.Name = "lblSeriousCaption";
            this.lblSeriousCaption.Size = new System.Drawing.Size(16, 18);
            this.lblSeriousCaption.TabIndex = 17;
            this.lblSeriousCaption.Text = "0";
            // 
            // lblCriticalCaption
            // 
            this.lblCriticalCaption.AutoSize = true;
            this.lblCriticalCaption.Location = new System.Drawing.Point(430, 21);
            this.lblCriticalCaption.Name = "lblCriticalCaption";
            this.lblCriticalCaption.Size = new System.Drawing.Size(16, 18);
            this.lblCriticalCaption.TabIndex = 18;
            this.lblCriticalCaption.Text = "0";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1354, 673);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.dgvAnimals);
            this.Controls.Add(this.grpSummaryReport);
            this.Controls.Add(this.grpFindAnimal);
            this.Controls.Add(this.grpAnimalDetails);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Baobab Ridge Wildlife Rehabilitation Records";
            this.grpAnimalDetails.ResumeLayout(false);
            this.grpAnimalDetails.PerformLayout();
            this.grpFindAnimal.ResumeLayout(false);
            this.grpFindAnimal.PerformLayout();
            this.grpSummaryReport.ResumeLayout(false);
            this.grpSummaryReport.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAnimals)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpAnimalDetails;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Label lblAnimalId;
        private System.Windows.Forms.GroupBox grpFindAnimal;
        private System.Windows.Forms.GroupBox grpSummaryReport;
        private System.Windows.Forms.DataGridView dgvAnimals;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnSaveChanges;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Label lblRecoveryScore;
        private System.Windows.Forms.Label lblAge;
        private System.Windows.Forms.Label lblSpecies;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Label lblSearchId;
        private System.Windows.Forms.Button btnGenerateSummary;
        private System.Windows.Forms.Label lblCritical;
        private System.Windows.Forms.Label lblReleaseReady;
        private System.Windows.Forms.Label lblRecovering;
        private System.Windows.Forms.Label lblStable;
        private System.Windows.Forms.Label lblSerious;
        private System.Windows.Forms.Label lblAverageScore;
        private System.Windows.Forms.Label lblAverageAge;
        private System.Windows.Forms.Label lblTotalAnimals;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.TextBox txtAge;
        private System.Windows.Forms.TextBox txtRecoveryScore;
        private System.Windows.Forms.TextBox txtSpecies;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.TextBox txtAnimalId;
        private System.Windows.Forms.TextBox txtSearchId;
        private System.Windows.Forms.Label lblCriticalCaption;
        private System.Windows.Forms.Label lblSeriousCaption;
        private System.Windows.Forms.Label lblStableCaption;
        private System.Windows.Forms.Label lblRecoveringCaption;
        private System.Windows.Forms.Label lblReleaseReadyCaption;
        private System.Windows.Forms.Label lblAverageScoreCaption;
        private System.Windows.Forms.Label lblAverageAgeCaption;
        private System.Windows.Forms.Label lblTotalAnimalsCaption;
    }
}


<?xml version="1.0" encoding="utf-8"?>
<root>
  <!-- 
    Microsoft ResX Schema 
    
    Version 2.0
    
    The primary goals of this format is to allow a simple XML format 
    that is mostly human readable. The generation and parsing of the 
    various data types are done through the TypeConverter classes 
    associated with the data types.
    
    Example:
    
    ... ado.net/XML headers & schema ...
    <resheader name="resmimetype">text/microsoft-resx</resheader>
    <resheader name="version">2.0</resheader>
    <resheader name="reader">System.Resources.ResXResourceReader, System.Windows.Forms, ...</resheader>
    <resheader name="writer">System.Resources.ResXResourceWriter, System.Windows.Forms, ...</resheader>
    <data name="Name1"><value>this is my long string</value><comment>this is a comment</comment></data>
    <data name="Color1" type="System.Drawing.Color, System.Drawing">Blue</data>
    <data name="Bitmap1" mimetype="application/x-microsoft.net.object.binary.base64">
        <value>[base64 mime encoded serialized .NET Framework object]</value>
    </data>
    <data name="Icon1" type="System.Drawing.Icon, System.Drawing" mimetype="application/x-microsoft.net.object.bytearray.base64">
        <value>[base64 mime encoded string representing a byte array form of the .NET Framework object]</value>
        <comment>This is a comment</comment>
    </data>
                
    There are any number of "resheader" rows that contain simple 
    name/value pairs.
    
    Each data row contains a name, and value. The row also contains a 
    type or mimetype. Type corresponds to a .NET class that support 
    text/value conversion through the TypeConverter architecture. 
    Classes that don't support this are serialized and stored with the 
    mimetype set.
    
    The mimetype is used for serialized objects, and tells the 
    ResXResourceReader how to depersist the object. This is currently not 
    extensible. For a given mimetype the value must be set accordingly:
    
    Note - application/x-microsoft.net.object.binary.base64 is the format 
    that the ResXResourceWriter will generate, however the reader can 
    read any of the formats listed below.
    
    mimetype: application/x-microsoft.net.object.binary.base64
    value   : The object must be serialized with 
            : System.Runtime.Serialization.Formatters.Binary.BinaryFormatter
            : and then encoded with base64 encoding.
    
    mimetype: application/x-microsoft.net.object.soap.base64
    value   : The object must be serialized with 
            : System.Runtime.Serialization.Formatters.Soap.SoapFormatter
            : and then encoded with base64 encoding.

    mimetype: application/x-microsoft.net.object.bytearray.base64
    value   : The object must be serialized into a byte array 
            : using a System.ComponentModel.TypeConverter
            : and then encoded with base64 encoding.
    -->
  <xsd:schema id="root" xmlns="" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:msdata="urn:schemas-microsoft-com:xml-msdata">
    <xsd:import namespace="http://www.w3.org/XML/1998/namespace" />
    <xsd:element name="root" msdata:IsDataSet="true">
      <xsd:complexType>
        <xsd:choice maxOccurs="unbounded">
          <xsd:element name="metadata">
            <xsd:complexType>
              <xsd:sequence>
                <xsd:element name="value" type="xsd:string" minOccurs="0" />
              </xsd:sequence>
              <xsd:attribute name="name" use="required" type="xsd:string" />
              <xsd:attribute name="type" type="xsd:string" />
              <xsd:attribute name="mimetype" type="xsd:string" />
              <xsd:attribute ref="xml:space" />
            </xsd:complexType>
          </xsd:element>
          <xsd:element name="assembly">
            <xsd:complexType>
              <xsd:attribute name="alias" type="xsd:string" />
              <xsd:attribute name="name" type="xsd:string" />
            </xsd:complexType>
          </xsd:element>
          <xsd:element name="data">
            <xsd:complexType>
              <xsd:sequence>
                <xsd:element name="value" type="xsd:string" minOccurs="0" msdata:Ordinal="1" />
                <xsd:element name="comment" type="xsd:string" minOccurs="0" msdata:Ordinal="2" />
              </xsd:sequence>
              <xsd:attribute name="name" type="xsd:string" use="required" msdata:Ordinal="1" />
              <xsd:attribute name="type" type="xsd:string" msdata:Ordinal="3" />
              <xsd:attribute name="mimetype" type="xsd:string" msdata:Ordinal="4" />
              <xsd:attribute ref="xml:space" />
            </xsd:complexType>
          </xsd:element>
          <xsd:element name="resheader">
            <xsd:complexType>
              <xsd:sequence>
                <xsd:element name="value" type="xsd:string" minOccurs="0" msdata:Ordinal="1" />
              </xsd:sequence>
              <xsd:attribute name="name" type="xsd:string" use="required" />
            </xsd:complexType>
          </xsd:element>
        </xsd:choice>
      </xsd:complexType>
    </xsd:element>
  </xsd:schema>
  <resheader name="resmimetype">
    <value>text/microsoft-resx</value>
  </resheader>
  <resheader name="version">
    <value>2.0</value>
  </resheader>
  <resheader name="reader">
    <value>System.Resources.ResXResourceReader, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value>
  </resheader>
  <resheader name="writer">
    <value>System.Resources.ResXResourceWriter, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value>
  </resheader>
  <metadata name="backgroundWorker1.TrayLocation" type="System.Drawing.Point, System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a">
    <value>17, 17</value>
  </metadata>
</root>
