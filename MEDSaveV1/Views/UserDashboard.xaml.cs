using System.Collections.ObjectModel;

namespace MEDSaveV1.Views;

public partial class UserDashboard : ContentPage
{
    public ObservableCollection<Appointment> Appointments { get; set; }
    private void OnSearchButtonPressed(object sender, EventArgs e)
    {
        var query = ServiceSearchBar.Text;

        // For now, let's just display the query. 
        // In a real-world app, you'd probably want to search for services using this query.
        DisplayAlert("Search", $"Searching for services related to: {query}", "OK");
    }

    public UserDashboard()
    {
        InitializeComponent();

        // For demonstration purposes, we'll use static data.
        // In a real-world application, you'd likely fetch this data from a service or database.

        NameLabel.Text = "Name: John Doe";
        DOBLabel.Text = "DOB: 15/04/1988";
        ContactLabel.Text = "Contact: +1-234-5678-910";
        EmailLabel.Text = "Email: johndoe@example.com";
        BloodTypeLabel.Text = "Blood Type: O+";
        KnownAllergiesLabel.Text = "Known Allergies: Peanuts";
        CurrentMedicationLabel.Text = "Current Medication: Aspirin, Metformin";

        Appointments = new ObservableCollection<Appointment>
            {
                new Appointment { AppointmentDate = "20/09/2023 10:00 AM", DoctorName = "Dr. Smith" },
                new Appointment { AppointmentDate = "25/09/2023 02:00 PM", DoctorName = "Dr. Jane" }
            };

        AppointmentsListView.ItemsSource = Appointments;
    }
}

public class Appointment
{
    public string AppointmentDate { get; set; }
    public string DoctorName { get; set; }

}