using System;
using System.Linq;
using System.Windows.Forms;
using TaskMaster.Model;

namespace TaskMaster
{
    public partial class Form1 : Form
    {
        private tmDBContext _tmDBContext;
        private const string Update = "Update";
        private const string Save = "Save";
        public Form1()
        {
            InitializeComponent();
            _tmDBContext = new tmDBContext();
            var statuses = _tmDBContext.Status.ToList();

            foreach (var status in statuses)
            {
                cboxStatus.Items.Add(status);
            }
            RefreshData();
        }

        private void RefreshData()
        {
            BindingSource binding = new BindingSource();

            var query = from t in _tmDBContext.Tasks
                        orderby t.DueDate
                        select new { t.Id, TaskName = t.Name, StatusName = t.Status.Name, t.DueDate };
            binding.DataSource = query.ToList();
            dgTaskTable.DataSource = binding;
            dgTaskTable.Refresh();
        }

        private void btnCreate_Click(object sender, System.EventArgs e)
        {
            TrySaveTask();
        }

        private bool TrySaveTask(int? id = null)
        {
            if (cboxStatus.SelectedItem == null || string.IsNullOrWhiteSpace(txtTask.Text))
            {
                MessageBox.Show("Need to have both a named task and selected status");
                return false;
            }

            var newTask = new Task
            {
                Id = id.HasValue ? id.Value : 0,
                Name = txtTask.Text,
                StatusId = (cboxStatus.SelectedItem as Status).Id,
                DueDate = pickerDueDate.Value,
            };

            if (id.HasValue)
            {
                var task = GetSelectedTask(Save);
                task.Name = newTask.Name;
                task.StatusId = newTask.StatusId;
                task.DueDate = newTask.DueDate;
            }
            else
            {
                _tmDBContext.Tasks.Add(newTask);
            }

            _tmDBContext.SaveChanges();
            RefreshData();
            return true;
        }

        private Task GetSelectedTask(string action)
        {
            if (dgTaskTable.SelectedCells == null || dgTaskTable.SelectedCells.Count == 0)
            {
                MessageBox.Show($"Need select task to {action}");
                return null;
            }
            var rowIndex = dgTaskTable.SelectedCells[0].RowIndex;

            return _tmDBContext.Tasks.Find((int)dgTaskTable.Rows[rowIndex].Cells[0].Value);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var task = GetSelectedTask(btnDelete.Text);
            if (task == null) return;

            _tmDBContext.Tasks.Remove(task);
            _tmDBContext.SaveChanges();

            RefreshData();
        }

        private void bthUpdate_Click(object sender, EventArgs e)
        {
            var task = GetSelectedTask(Update);
            if (task == null) return;

            if (btnUpdate.Text == Update)
            {
                txtTask.Text = task.Name;
                pickerDueDate.Value = task.DueDate.HasValue ? task.DueDate.Value : DateTime.Now;
                foreach (Status s in cboxStatus.Items)
                {
                    if (s.Name == task.Status.Name)
                    {
                        cboxStatus.SelectedItem = s;
                        break;
                    }
                }
                btnUpdate.Text = Save;
                return;
            }

            if (TrySaveTask(task.Id))
            {
                btnUpdate.Text = Update;
            }
        }
    }
}
