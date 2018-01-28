using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SQLtut1
{
    public partial class BizContacts : Form
    {
        string connString = @"Data Source=DESKTOP-BC1H151\SQLEXPRESS;Initial Catalog=AdressBook;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        SqlDataAdapter dataAdapter;  //allows as to build connection between program to database
        DataTable table;//table hold data to grid
        SqlCommandBuilder commandBuilder;//declare Sql commandBuilder
        SqlConnection conn;//value to hold sql connection
        string selectionStatement = "Select * from BizContacts";

        public BizContacts()
        {
            InitializeComponent();
        }

        private void BizContacts_Load(object sender, EventArgs e)
        {
            cboSearch.SelectedIndex = 0;//first item is selected
            dataGridView1.DataSource = bindingSource1;//set the source of inventory encapsulate the data for the form

            GetData(selectionStatement);//calls method called getdata, represent an sql query
        }

        private void GetData(string selectCommand)
        {
            try
            {
                dataAdapter = new SqlDataAdapter(selectCommand, connString);//pass in the select command and connection string
                table = new DataTable();//make a new data table object
                table.Locale = System.Globalization.CultureInfo.InvariantCulture;
                dataAdapter.Fill(table);//fill the data table
                bindingSource1.DataSource = table;//set the datasource on the binding source on the table
                dataGridView1.Columns[0].ReadOnly = true;//this help prevent the id field from being change
            }
            catch(SqlException ex)
            {
                MessageBox.Show(ex.Message);//show usefull message to the user
            }
        }
        //add
        private void btnAdd_Click(object sender, EventArgs e)
        {
            SqlCommand command;//declares a new sql command object
            //field names in the table
            string insert = @"insert into BizContacts(Date_Added, Company, Website, Title, First_Name, Last_Name, Address,
                                                      City, State, Postal_Code, Mobile, Notes)
                              values(@Date_Added, @Company, @Website, @Title, @First_Name, @Last_Name, @Address,
                                                      @City, @State, @Postal_Code, @Mobile, @Notes)";

            using (conn = new SqlConnection(connString)) //allows disposing low lvl resources
            {
                try {
                    conn.Open();//oppening connection
                    command = new SqlCommand(insert, conn);
                    command.Parameters.AddWithValue(@"Date_Added", dateTimePicker1.Value.Date);//read value from form datePicker to table
                    command.Parameters.AddWithValue(@"Company", txtCompany.Text);
                    command.Parameters.AddWithValue(@"Website", txtWebsite.Text);
                    command.Parameters.AddWithValue(@"Title", txtTitle.Text);
                    command.Parameters.AddWithValue(@"First_Name", txtFName.Text);
                    command.Parameters.AddWithValue(@"Last_name", txtLName.Text);
                    command.Parameters.AddWithValue(@"Address", txtAddress.Text);
                    command.Parameters.AddWithValue(@"City", txtCity.Text);
                    command.Parameters.AddWithValue(@"State", txtState.Text);
                    command.Parameters.AddWithValue(@"Postal_Code", txtPCode.Text);
                    command.Parameters.AddWithValue(@"Mobile", txtMobile.Text);
                    command.Parameters.AddWithValue(@"Notes", txtNotes.Text);

                    command.ExecuteNonQuery();//push stuff into the table
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message); //if something wrong,show the user error
                }

            }
            GetData(selectionStatement);
            dataGridView1.Update();//update the view in data grid
        }
        //edit
        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            commandBuilder = new SqlCommandBuilder(dataAdapter);
            dataAdapter.UpdateCommand = commandBuilder.GetUpdateCommand();//get the update command
            try
            {
                bindingSource1.EndEdit();//update the table that is in memmory of the program
                dataAdapter.Update(table);//actually update the database
                MessageBox.Show("Update Successfull");//show the user that the database is updated
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //delete
        private void btnDel_Click(object sender, EventArgs e)
        {
            DataGridViewRow row = dataGridView1.CurrentCell.OwningRow;//grab a reference to the current row
            string value = row.Cells["ID"].Value.ToString();//grab value from the ID field of the selected record
            string fname = row.Cells["First_Name"].Value.ToString();
            string lname = row.Cells["Last_Name"].Value.ToString();
            DialogResult result = MessageBox.Show("Do you really want to delete?"+fname+" "+lname+",record" +value,"Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            string deleteState = @"Delete From BizContacts where id = '" + value + "'";//sql to delete

            if(result==DialogResult.Yes)
            {
                using (conn = new SqlConnection(connString))
                {
                    try
                    {
                        conn.Open();
                        SqlCommand comm = new SqlCommand(deleteState, conn);
                        comm.ExecuteNonQuery();
                        GetData(selectionStatement);//Refresh or get data again
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            switch (cboSearch.SelectedItem.ToString())
            {
                case "First Name":
                    GetData("Select * from BizContacts where lower(first_name) like '%" + txtSearch.Text.ToLower() + "%'");
                break;

                case "Last Name":
                    GetData("Select * from BizContacts where lower(last_name) like '%" + txtSearch.Text.ToLower() + "%'");
                    break;

                case "Company":
                    GetData("Select * from BizContacts where lower(company) like '%" + txtSearch.Text.ToLower() + "%'");
                    break;
            }
        }
    }
}
