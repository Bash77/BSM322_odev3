using gorsel_programlam_odev2;
namespace gorsel_programlam_odev2
{
    public class NoteDetailsPage : ContentPage
    {
        public NoteDetailsPage(Note selectedNote)
        {

            var titleLabel = new Label
            {
                Text = selectedNote.Title,
                FontSize = 20,
                FontAttributes = FontAttributes.Bold,
                Margin = new Thickness(10)
            };

            var detailsLabel = new Label
            {
                Text = selectedNote.Details,
                FontSize = 16,
                Margin = new Thickness(10)
            };


            Content = new StackLayout
            {
                Children = { titleLabel, detailsLabel }
            };
        }
        private void OnDarkModeToggled(object sender, ToggledEventArgs e)
        {
            App.ToggleTheme(e.Value); 
        }
    }
}