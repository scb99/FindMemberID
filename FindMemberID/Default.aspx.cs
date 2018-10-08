// ***********************************************************************
// Assembly         : FindMemberID
// Author           : Steve Bruell
// Created          : 10-02-2018
//
// Last Modified By : Steve Bruell
// Last Modified On : 10-03-2018
// ***********************************************************************
// <copyright file="Default.aspx.cs" company="SCB">
//     SCB. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************using System;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;
using StpcDB;
using StpcDBConnection;
using StpcModels;

namespace FindMemberID
{
    public partial class _Default : Page
    {
        private List<Member> allMembers;
        private List<Member> memberListToDisplay;

        public _Default()
        {
            DB.Instance.ConnectionString = Config.ConnectionString; //ConfigurationManager.ConnectionStrings["StpcDB"].ConnectionString + "3";

            this.allMembers = this.memberListToDisplay = this.GetAll();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsCallback)
            {
                if (this.filterTextBox.Text != null)
                {
                    this.UpdateDisplayedMembersByLastName(this.filterTextBox.Text);
                }

                this.usersGridView.DataSource = this.DisplayMembers;

                this.usersGridView.DataBind();
            }
        }

        public List<Member> DisplayMembers
        {
            get { return this.memberListToDisplay; }

            set { this.memberListToDisplay = value; }
        }

        public List<Member> GetAll()
        {
            return new List<Member>(DB.Instance.GetAllMembers<SqlMember>());
        }

        protected void UserSelected_Click(object sender, EventArgs e)
        {
            int id = Convert.ToInt32((sender as LinkButton).CommandArgument, CultureInfo.CurrentCulture);

            string displayText = "Nothing selected";

            foreach (Member member in this.DisplayMembers)
            {
                if (member.ID == id)
                {
                    displayText = id + " R " + member.LastName + ", " + member.FirstName;
                    break;
                }
            }

            Page.ClientScript.RegisterStartupScript(
                   Page.GetType(),
                   "MessageBox",
                   "<script language='javascript'>prompt('Press ctrl+C to select text in box below', '" + displayText + "');</script>");
        }

        protected void FilterTextBox_TextChanged(object sender, EventArgs e)
        {
            this.UpdateDisplayedMembersByLastName((sender as TextBox).Text);

            Page.SetFocus(filterTextBox);
        }

        private bool ShouldMemberBeIncluded(string nameOrID, string filter)
        {
            return nameOrID.ToLower(CultureInfo.CurrentCulture).StartsWith(filter.ToLower(CultureInfo.CurrentCulture), StringComparison.CurrentCulture);
        }

        private void UpdateDisplayedMembersByLastName(string filter)
        {
            this.DisplayMembers = filter != null && string.IsNullOrEmpty(filter) ? this.allMembers : this.allMembers.FindAll((member) => this.ShouldMemberBeIncluded(member.LastName, filter));
        }
    }
}