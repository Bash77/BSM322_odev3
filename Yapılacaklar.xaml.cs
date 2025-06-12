using Firebase.Database;
using Xamarin.Essentials;
using System;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Database.Query;

namespace gorsel_programlam_odev2
{
    public partial class YapılacaklarPage : ContentPage
    {
        FirebaseClient firebase;
        Note selectedNote;

        public YapılacaklarPage()
        {
            InitializeComponent();
            firebase = new FirebaseClient("https://first-project-9fa47-default-rtdb.firebaseio.com/");

            LoadNotes();

        }
        private void OnDarkModeToggled(object sender, ToggledEventArgs e)
        {
            App.ToggleTheme(e.Value);
        }
        async void OnSaveClicked(object sender, EventArgs e)
        {

            try
            {
                Console.WriteLine("Save button clicked");

                var title = titleEntry.Text.Trim();
                var details = detailsEntry.Text;

                if (string.IsNullOrWhiteSpace(title))
                {
                    await DisplayAlert("hata", "Başlık boş olamaz", "tamam");
                    return;
                }

                var note = new Note
                {
                    Title = title,
                    Details = details,
                    DateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                };

                Console.WriteLine("Before PostAsync");
                await firebase.Child("notes").PostAsync(note);
                Console.WriteLine("After PostAsync");
                bool result = await DisplayAlert("Onayla", "kaydet etmeyi emin misiniz", "evet", "hayir");

                await DisplayAlert("basarli", "Notu basarli kaydeildi", "tamam");

                Console.WriteLine("Notu basarli kadeedildi");

                titleEntry.Text = string.Empty;
                detailsEntry.Text = string.Empty;

                Console.WriteLine("Before LoadNotes");
                await LoadNotes();
                Console.WriteLine("After LoadNotes");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in OnSaveClicked: {ex.Message}");
                await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
        }

        private void AddNoteToListView(Note note)
        {
            var existingNotes = (List<Note>)notesListView.ItemsSource;

            if (existingNotes == null)
                existingNotes = new List<Note>();

            existingNotes.Add(note);

            notesListView.ItemsSource = existingNotes;
        }


        async void OnNoteSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;

            var selectedNote = (Note)e.SelectedItem;

            await Navigation.PushAsync(new NoteDetailsPage(selectedNote));

            ((ListView)sender).SelectedItem = null;
        }

        async void OnCancelClicked(object sender, EventArgs e)
        {
            bool result = await DisplayAlert("Onayla", "iptal etmeyi emin misiniz", "evet", "hayir");

            if (result)
            {
                titleEntry.Text = string.Empty;
                detailsEntry.Text = string.Empty;
            }
        }

        async void OnDeleteClicked(object sender, EventArgs e)
        {
            try
            {
                var button = (Button)sender;
                var noteToDelete = (Note)button.CommandParameter;

                bool result = await DisplayAlert("Onayla", "Bu notu silmek istediğinizden emin misiniz?", "Evet", "Hayir");

                if (result)
                {
                    var existingNotes = (List<Note>)notesListView.ItemsSource;
                    existingNotes.Remove(noteToDelete);
                    notesListView.ItemsSource = null;
                    notesListView.ItemsSource = existingNotes;

                    await firebase.Child("notes").OnceAsync<Note>();
                    var noteToDeleteFirebase = (await firebase.Child("notes").OnceAsync<Note>())
                        .FirstOrDefault(x => x.Object.Title == noteToDelete.Title && x.Object.DateTime == noteToDelete.DateTime);

                    if (noteToDeleteFirebase != null)
                    {
                        await firebase.Child("notes").Child(noteToDeleteFirebase.Key).DeleteAsync();
                    }

                    await DisplayAlert("basarli", "Notu basarli silindi", "tamam");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in OnDeleteClicked: {ex.Message}");
                await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
        }

        private async void OnEditClicked(object sender, EventArgs e)
        {
            try
            {
                var button = (Button)sender;
                var noteToEdit = (Note)button.CommandParameter;

                await Navigation.PushAsync(new EditPage(noteToEdit));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in OnEditClicked: {ex.Message}");
                await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
        }

        async Task LoadNotes()
        {
            var notes = await firebase.Child("notes").OnceAsync<Note>();
            notesListView.ItemsSource = notes.Select(x => x.Object).ToList();
        }

    }
    }
