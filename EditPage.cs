using Firebase.Database;
using Firebase.Database.Query;
using gorsel_programlam_odev2; 


public class EditPage : ContentPage
{
    private Note _editedNote;
    private Entry titleEntry;
    private Entry detailsEntry;
    private FirebaseClient firebase;

    public EditPage(Note note)
    {
        _editedNote = note;
        firebase = new FirebaseClient("https://first-project-9fa47-default-rtdb.firebaseio.com/");


        titleEntry = new Entry
        {
            Placeholder = "Title",
            Text = _editedNote.Title
        };

        detailsEntry = new Entry
        {
            Placeholder = "Details",
            Text = _editedNote.Details
        };

        var saveButton = new Button
        {
            Text = "Save",
            TextColor = Color.FromHex("#FFFFFF"),
            BackgroundColor = Color.FromHex("#3498db") 
        };


        saveButton.Clicked += async (s, e) => await OnSaveEditClicked();

        Content = new StackLayout
        {
            Children = { titleEntry, detailsEntry, saveButton }
        };
    }
   
    private async Task OnSaveEditClicked()
    {
        try
        {
            _editedNote.Title = titleEntry.Text.Trim();
            _editedNote.Details = detailsEntry.Text;

            await firebase.Child("notes").Child(_editedNote.Title).PutAsync(_editedNote);

            await LoadNotes();

            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in OnSaveEditClicked: {ex.Message}");
            await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
        }
    }


    private async Task LoadNotes()
    {
        var notes = await firebase.Child("notes").OnceAsync<Note>();
        
    }
}
