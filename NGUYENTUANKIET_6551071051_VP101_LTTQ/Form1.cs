
using Microsoft.EntityFrameworkCore;
using NGUYENTUANKIET_6551071051_VP101_LTTQ.Models.EF;
using NGUYENTUANKIET_6551071051_VP101_LTTQ.Models.Entities;

namespace NGUYENTUANKIET_6551071051_VP101_LTTQ
{
    public partial class Form1 : Form
    {
        private readonly string connectionString =
            @"Server=.\SQLEXPRESS;Database=DuLieu;Trusted_Connection=True;TrustServerCertificate=True;";

        private string? maDangChon = null;
        private bool dangXuLy = false;

        public Form1()
        {
            InitializeComponent();
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.AutoGenerateColumns = true;
        }

        private async Task LoadDuLieuAsync()
        {
               try
              {
                var options = new DbContextOptionsBuilder<AppDbContext>()
                    .UseSqlServer(connectionString)
                    .Options;
                using var db = new AppDbContext(options);
                var danhSach = await db.CongViecs
                    .AsNoTracking()
                    .ToListAsync();
                dataGridView1.DataSource = null;
                dataGridView1.DataSource = danhSach;
                dataGridView1.AutoSizeColumnsMode =
                    DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
              MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message);
            }
        }

        private void dataGridView1_CellContentClick(
            object sender, DataGridViewCellEventArgs e)
        {
            if (dangXuLy || e.RowIndex < 0) return;
            if (e.RowIndex >= dataGridView1.Rows.Count) return;

            var row = dataGridView1.Rows[e.RowIndex];

            if (row.IsNewRow) return;

            maDangChon =
                row.Cells["MaCongViec"].Value?.ToString();
            textBox1.Text =
                row.Cells["MaCongViec"].Value?.ToString() ?? "";
            textBox2.Text =
                row.Cells["TenCongViec"].Value?.ToString() ?? "";
            textBox3.Text =
                row.Cells["LuongThang"].Value?.ToString() ?? "";
            TrangThaiDaChon();
        }

        private void TrangThaiBanDau()
        {
            maDangChon = null;
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox1.Enabled = true;
            button1.Enabled = true;
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = true;
            dataGridView1.ClearSelection();
        }

        private void TrangThaiDaChon()
          {
            textBox1.Enabled = false;
            button1.Enabled = false;
            button2.Enabled = true;
            button3.Enabled = true;
            button4.Enabled = true;
        }

        private void KhoaNut()
        {
            dangXuLy = true;
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;
            dataGridView1.Enabled = false;
           }

        private void MoKhoaNut()
        {
            dangXuLy = false;
            dataGridView1.Enabled = true;

               if (maDangChon == null)
             {
                button1.Enabled = true;
                button2.Enabled = false;
                button3.Enabled = false;
                button4.Enabled = true;
                textBox1.Enabled = true;
            }
            else
            {
                TrangThaiDaChon();
            }
           }

            private bool KiemTraDuLieu()
           {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
              {
                  MessageBox.Show("Vui lòng nhập mã công việc!");
                textBox1.Focus();
                return false;
              }

            if (textBox1.Text.Trim().Length > 10)
              {
                 MessageBox.Show("Mã công việc tối đa 10 ký tự!");
                textBox1.Focus();
                return false;
               }

            if (string.IsNullOrWhiteSpace(textBox2.Text))
              {
                MessageBox.Show("Vui lòng nhập tên công việc!");
                 textBox2.Focus();
                return false;
               }

            if (textBox2.Text.Trim().Length > 50)
               {
                MessageBox.Show("Tên công việc tối đa 50 ký tự!");
                textBox2.Focus();
                 return false;
               }

            if (!decimal.TryParse(textBox3.Text, out decimal luong)
                || luong <= 0)
              {
                MessageBox.Show("Lương tháng phải lớn hơn 0!");
                 textBox3.Focus();
                return false;
               }

            if (luong > 9999999999999999.99m)
              {
              MessageBox.Show("Lương tháng vượt quá giới hạn!");
                textBox3.Focus();
                return false;
              }
            return true;
          }

        private async void Form1_Load(object sender, EventArgs e)
        {
             TrangThaiBanDau();
            await LoadDuLieuAsync();
            TrangThaiBanDau();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            if (dangXuLy) return;
             if (!KiemTraDuLieu()) return;
            KhoaNut();
            try
               {
                var options = new DbContextOptionsBuilder<AppDbContext>()
                    .UseSqlServer(connectionString)
                    .Options;
                using var db = new AppDbContext(options);
                string ma = textBox1.Text.Trim();
                bool daTonTai = await db.CongViecs
                    .AnyAsync(x => x.MaCongViec == ma);
                if (daTonTai)
                {
                    MessageBox.Show("Mã công việc đã tồn tại!");
                    return;
                }

                var cv = new CongViec
                {
                    MaCongViec = ma,
                    TenCongViec = textBox2.Text.Trim(),
                    LuongThang = decimal.Parse(textBox3.Text)
                };

                db.CongViecs.Add(cv);
                await db.SaveChangesAsync();
                MessageBox.Show("Thêm thành công!");
                await LoadDuLieuAsync();
                TrangThaiBanDau();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi thêm: " + ex.Message);
            }
            finally
            {
                MoKhoaNut();
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            if (dangXuLy) return;
            if (maDangChon == null)
            {
                MessageBox.Show("Vui lòng chọn công việc cần sửa!");
                return;
            }
           if (!KiemTraDuLieu()) return;
            KhoaNut();
            try
            {
                var options = new DbContextOptionsBuilder<AppDbContext>()
                    .UseSqlServer(connectionString)
                    .Options;
                using var db = new AppDbContext(options);
                var cv = await db.CongViecs.FindAsync(maDangChon);
                if (cv == null)
                {
                  MessageBox.Show("Không tìm thấy công việc!");
                   maDangChon = null;
                  await LoadDuLieuAsync();
                    TrangThaiBanDau();
                    return;
                }
                cv.TenCongViec = textBox2.Text.Trim();
                cv.LuongThang = decimal.Parse(textBox3.Text);
                await db.SaveChangesAsync();
                MessageBox.Show("Sửa thành công!");
                await LoadDuLieuAsync();
                TrangThaiBanDau();
            }
          catch (Exception ex)
           {
                MessageBox.Show("Lỗi sửa: " + ex.Message);
            }
          finally
            {
                MoKhoaNut();
            }
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            if (dangXuLy) return;

         if (maDangChon == null)
            {
                MessageBox.Show("Vui lòng chọn công việc cần xóa");
                return;
            }
            var kq = MessageBox.Show(
                "Bạn có chắc muốn xóa công việc này ko",
        "Xác nhận",
                MessageBoxButtons.YesNo,
           MessageBoxIcon.Question);
            if (kq != DialogResult.Yes) return;
            KhoaNut();
            try
            {
                var options = new DbContextOptionsBuilder<AppDbContext>()
                    .UseSqlServer(connectionString)
                    .Options;
                using var db = new AppDbContext(options);
                var cv = await db.CongViecs.FindAsync(maDangChon);
                if (cv == null)
                  {
                    MessageBox.Show("Không tìm thấy công việc!");
                    maDangChon = null;
                    await LoadDuLieuAsync();
                    TrangThaiBanDau();
                    return;
                  }
  
                db.CongViecs.Remove(cv);
                await db.SaveChangesAsync();
                MessageBox.Show("Xóa thành công!");
                await LoadDuLieuAsync();
                TrangThaiBanDau();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi xóa: " + ex.Message);
             }
             finally
             {
                MoKhoaNut();
              }
        }
        private void button4_Click(object sender, EventArgs e)
        {
            if (dangXuLy) return;
            var kq = MessageBox.Show(
                "Bạn có  muốn thóat ko  ",
        "Xác nhận",
                MessageBoxButtons.YesNo,
           MessageBoxIcon.Question);
            this.Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
        }
    }
}
