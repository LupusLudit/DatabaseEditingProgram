﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseEditingProgram.database.databaseEntities
{
    public class Order : IDatabaseEntity
    {
        private int id;
        private Customer customer;
        private Book book;
        private DateTime date;
        private DateTime time;

        public int ID { get { return id; } set { id = value; } }
        public Customer Customer { get { return customer; } set { customer = value; } }
        public Book Book { get { return book; } set { book = value; } }
        public DateTime Date { get { return date; } set { date = value; } }
        public DateTime Time { get { return time; } set { time = value; } }

        public Order(int id, Customer customer, Book book, DateTime date, DateTime time) 
        {
            this.id = id;
            this.customer = customer;
            this.book = book;
            this.date = date;
            this.time = time;
        }

        public Order(Customer customer, Book book, DateTime date, DateTime time)
        {
            this.id = 0;
            this.customer = customer;
            this.book = book;
            this.date = date;
            this.time = time;
        }
    }
}
