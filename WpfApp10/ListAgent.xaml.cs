﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WpfApp10
{
    /// <summary>
    /// Логика взаимодействия для ListAgent.xaml
    /// </summary>
    public partial class ListAgent : Page
    {
        private int start = 0;
        private int fullCount = 0;
        private int order = 0;
        private int iag = 0;
        private string fnd = "";
        Frame frame;

        public ListAgent(Frame frame)
        {
            InitializeComponent();
            this.frame = frame;
            List<AgentType> agents = new List<AgentType> { };
            agents = helper.GetContext().AgentType.ToList();
            agents.Add(new AgentType { Title = "Все типы" });
            TypeCmb.ItemsSource = agents.OrderBy(AgentType => AgentType.ID);
            Load();
        }

        public void Load()
        {

            try
            {
                var ag = helper.GetContext().Agent.Where(Agent => Agent.Title.Contains(fnd) || Agent.Phone.Contains(fnd) || Agent.Email.Contains(fnd));
                if (iag == 0)
                {
                    fullCount = ag.Count();
                    if (order == 0) agentGrid.ItemsSource = ag.OrderBy(Agent => Agent.ID).Skip(start * 10).Take(10).ToList();
                    if (order == 1) agentGrid.ItemsSource = ag.OrderBy(Agent => Agent.Title).Skip(start * 10).Take(10).ToList();
                    if (order == 2) agentGrid.ItemsSource = ag.OrderByDescending(Agent => Agent.Title).Skip(start * 10).Take(10).ToList();
                    if (order == 3) agentGrid.ItemsSource = ag.OrderBy(Agent => Agent.Priority).Skip(start * 10).Take(10).ToList();
                    if (order == 4) agentGrid.ItemsSource = ag.OrderByDescending(Agent => Agent.Priority).Skip(start * 10).Take(10).ToList();
                }
                else
                {
                    var agg = ag.Where(Agent => Agent.AgentTypeID == iag);
                    fullCount = agg.Count();
                    if (order == 0) agentGrid.ItemsSource = agg.OrderBy(Agent => Agent.ID).Skip(start * 10).Take(10).ToList();
                    if (order == 1) agentGrid.ItemsSource = agg.OrderBy(Agent => Agent.Title).Skip(start * 10).Take(10).ToList();
                    if (order == 2) agentGrid.ItemsSource = agg.OrderByDescending(Agent => Agent.Title).Skip(start * 10).Take(10).ToList();
                    if (order == 3) agentGrid.ItemsSource = agg.OrderBy(Agent => Agent.Priority).Skip(start * 10).Take(10).ToList();
                    if (order == 4) agentGrid.ItemsSource = agg.OrderByDescending(Agent => Agent.Priority).Skip(start * 10).Take(10).ToList();
                }
            }
            catch
            {
                return;
            };


            full.Text = fullCount.ToString();
            int ost = fullCount % 10;
            int pag = (fullCount - ost) / 10;
            if (ost > 0) pag++;
            pagin.Children.Clear();
            for (int i = 0; i < pag; i++)
            {
                Button myButton = new Button();
                myButton.Height = 30;
                myButton.Content = i + 1;
                myButton.Width = 20;
                myButton.HorizontalAlignment = HorizontalAlignment.Center;
                myButton.Tag = i;
                myButton.Click += new RoutedEventHandler(paginButto_Click); ;
                pagin.Children.Add(myButton);
            }
            turnButton();

        }


        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void updateButton_Click(object sender, RoutedEventArgs e)
        {
            if (agentGrid.SelectedItems.Count > 0)
            {
                int prt = 0;
                foreach (Agent agent in agentGrid.SelectedItems)
                {
                    if (agent.Priority > prt) prt = agent.Priority;
                }
                PriorityWindow dlg = new PriorityWindow(prt);
                helper.prioritet = prt;
                helper.flag = false;
                dlg.ShowDialog();
                if (helper.flag)
                {
                    foreach (Agent agent in agentGrid.SelectedItems)
                    {
                        agent.Priority = helper.prioritet;
                        helper.GetContext().Entry(agent).State = EntityState.Modified;
                    }
                    helper.GetContext().SaveChanges();
                    Load();
                }
            }


        }

        private void prevButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            frame.Content = new AddAgent(null);
        }
        private void paginButto_Click(object sender, RoutedEventArgs e)
        {
            start = Convert.ToInt32(((Button)sender).Tag.ToString());
            turnButton();
            Load();


        }
        private void turnButton()
        {
            if (start == 0) { back.IsEnabled = false; }
            else { back.IsEnabled = true; };
            if ((start + 1) * 10 > fullCount) { forward.IsEnabled = false; }
            else { forward.IsEnabled = true; };
        }


        private void back_Click(object sender, RoutedEventArgs e)
        {
            start--;
            turnButton();
            Load();

        }

        private void forward_Click(object sender, RoutedEventArgs e)
        {
            start++;
            turnButton();
            Load();

        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            ComboBoxItem selectedItem = (ComboBoxItem)comboBox.SelectedItem;
            order = Convert.ToInt32(selectedItem.Tag.ToString());
            Load();

        }

        private void TypeCmb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            iag = ((AgentType)TypeCmb.SelectedItem).ID;
            Load();

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            fnd = ((TextBox)sender).Text;
            Load();

        }

        private void agentGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (agentGrid.SelectedItems.Count > 0)
            {
                Agent agent = agentGrid.SelectedItems[0] as Agent;

                if (agent != null)
                {
                    frame.Content = new AddAgent(agent);
                }
            }

        }

        private void agentGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            Agent agent = (Agent)e.Row.DataContext;
            int sum = 0;
            double fsum = 0;
            foreach (ProductSale ps in agent.ProductSale)
            {
                List<ProductMaterial> mtr = new List<ProductMaterial> { };
                mtr = helper.GetContext().ProductMaterial.Where(ProductMaterial => ProductMaterial.ProductID == ps.ProductID).ToList();
                foreach (ProductMaterial mt in mtr)
                {
                    double f = decimal.ToDouble(mt.Material.Cost);
                    fsum += f * (double)mt.Count;
                }
                fsum = fsum * ps.ProductCount;
                if (ps.SaleDate.AddDays(365).CompareTo(DateTime.Today) > 0)
                    sum += ps.ProductCount;
            }
            agent.sale = sum;
            agent.percent = 0;
            if (fsum >= 10000 && fsum < 50000) agent.percent = 5;
            if (fsum >= 50000 && fsum < 150000) agent.percent = 10;
            if (fsum >= 150000 && fsum < 500000) agent.percent = 20;
            if (fsum >= 500000) {
                agent.percent = 25;
                SolidColorBrush hb = new SolidColorBrush(Colors.LightGreen);
                e.Row.Background = hb;
            }

        }

    }
}
