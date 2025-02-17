# DatabaseEditingProgram

This project is a database editing tool designed to provide an intuitive and efficient way to manage a fictional Book store database.
Once connected, you will be able to edit, delete or add new records.

## Requirments

* Visual Studio (or other code editing program)
* NET 8.0 (or compatible version)
* Ensure your software can run the WPF c# framework

## How to connect? - Tutorial

First you need to clone this repository:

```
https://github.com/LupusLudit/DatabaseEditingProgram.git
```

Then simply run this program in Visual Studio.
A new window will pop up. You need to input your login info here and then click on the "Connect" button.
If the connecting process is taking too long, consider closing the connection by clicking on the "Close" button (if you are not connecting to the database the "Close" button will simply close the program).
If you have successfully connected to your database a new window will pop up.
This window will notify you about the dangers of this program and how it can affect your database. If you want to proceed, you need to click on "Yes".
Again, a new window will show up. This time it is the actual database. You are connected!

## How to work with the database?
Under each table there should be a "Add new" button. Try clicking on it. A new record should appear.
Note that if you want to create a new book record there has to be at least one genre and one publisher in the database.
If you want to create a new purchase at least one book and one customer has to be in the database.

### Editing and deleting
You can edit most attributes in your table records. First you need to double click on the cell with the attribute you want to edit. 
Then insert the values.
ATTENTION: If you want to save your record, you need to first hit "Enter" on your keyboard to confirm the option. Then click on the "Save" button next to the record to save your data to the database.
Deleting is simple. Just click on the "Delete" button. Beware that this action can't be undone.
Note that when deleting a record which is bound to a different "higher" record (for example deleting a genre bound to a book) both records will be removed from the database.
Sometimes after editing a "bound" table it is neccessery to reload the "dependent" table by clicking on the "Refresh" button.

## Importing and exporting
Some tables allow data for import/export. This is done simply by clicking on the buttons below the table ("Import CSV", "Export CSV").

## Final note
I hope you will find my project usefull and that you will enjoy using it.
Extra note: This program is being used as a school project
