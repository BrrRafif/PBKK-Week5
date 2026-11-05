using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Data.SqlClient;
using StudentManager.Data;
using StudentManager.Models;

namespace StudentManager.ViewModels;

public class StudentViewModel : INotifyPropertyChanged
{
    private readonly StudentRepository _repository;

    public ObservableCollection<Student> Students { get; } = new();

    private Student? _selectedStudent;
    public Student? SelectedStudent
    {
        get => _selectedStudent;
        set
        {
            _selectedStudent = value;
            OnPropertyChanged();
            if (value != null)
                LoadToForm(value);
        }
    }

    // Data yang sedang diisi di form (salinan, bukan objek di DataGrid)
    private Student _formStudent = new();
    public Student FormStudent
    {
        get => _formStudent;
        set
        {
            _formStudent = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsEditMode));
            OnPropertyChanged(nameof(SaveButtonText));
            OnPropertyChanged(nameof(FormModeText));
        }
    }

    // Tampilan form: mode tambah atau edit
    public bool IsEditMode => FormStudent.id != 0;
    public string SaveButtonText => IsEditMode ? "Update" : "Simpan";
    public string FormModeText => IsEditMode ? $"Mode edit: NRP {FormStudent.NRP}" : "Mode tambah data baru";

    // Pesan validasi (merah) dan pesan berhasil (hijau) di bawah form
    private string _errorMessage = "";
    public string ErrorMessage
    {
        get => _errorMessage;
        set { _errorMessage = value; OnPropertyChanged(); }
    }

    private string _successMessage = "";
    public string SuccessMessage
    {
        get => _successMessage;
        set { _successMessage = value; OnPropertyChanged(); }
    }

    private string _searchText = "";
    public string SearchText
    {
        get => _searchText;
        set
        {
            _searchText = value;
            OnPropertyChanged();
        }
    }

    // Statistik
    public int TotalStudents => Students.Count;

    // 6 jurusan / departemen
    public int TotalTeknikInformatika => CountJurusan("Teknik Informatika");
    public int TotalTeknikElektro => CountJurusan("Teknik Elektro");
    public int TotalTeknikBiomedik => CountJurusan("Teknik Biomedik");
    public int TotalTeknikKomputer => CountJurusan("Teknik Komputer");
    public int TotalTeknologiInformasi => CountJurusan("Teknologi Informasi");
    public int TotalSistemInformasi => CountJurusan("Sistem Informasi");

    // Gender
    public int TotalLakiLaki =>
        Students.Count(x => x.Gender == "Laki-laki");
    public int TotalPerempuan =>
        Students.Count(x => x.Gender == "Perempuan");

    private int CountJurusan(string jurusan) =>
        Students.Count(x => x.Jurusan == jurusan);

    public ICommand SaveCommand { get; }
    public ICommand ResetCommand { get; }
    public ICommand SearchCommand { get; }
    public ICommand ResetSearchCommand { get; }
    public ICommand EditRowCommand { get; }
    public ICommand DeleteRowCommand { get; }

    public StudentViewModel()
    {
        _repository = new StudentRepository();
        LoadData();

        SaveCommand = new RelayCommand(Save);
        ResetCommand = new RelayCommand(Reset);
        SearchCommand = new RelayCommand(Search);
        ResetSearchCommand = new RelayCommand(ResetSearch);
        EditRowCommand = new RelayCommand(p =>
        {
            if (p is Student s) LoadToForm(s);
        });
        DeleteRowCommand = new RelayCommand(p =>
        {
            if (p is Student s) DeleteByid(s.id);
        });

        Reset();
    }

    private void LoadData()
    {
        Students.Clear();

        foreach (var student in _repository.GetAll())
            Students.Add(student);

        RefreshStatistics();
    }

    private void Save()
    {
        SuccessMessage = "";

        // VALIDATOR
        string? error = Validate(FormStudent);
        if (error != null)
        {
            ErrorMessage = error;
            return;
        }

        try
        {
            bool isNew = FormStudent.id == 0;

            if (isNew)
                _repository.Insert(FormStudent);
            else
                _repository.Update(FormStudent);

            LoadData();
            Reset();
            SuccessMessage = isNew
                ? "✔ Data mahasiswa berhasil disimpan."
                : "✔ Data mahasiswa berhasil diperbarui.";
        }
        catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
        {
            // Cadangan: ditolak oleh UNIQUE constraint di database
            ErrorMessage = $"NRP {FormStudent.NRP} sudah terdaftar. NRP tidak boleh sama.";
        }
    }

    // VALIDATOR sederhana:
    // 1. Semua kolom wajib diisi
    // 2. NRP tidak boleh sama dengan mahasiswa lain (unique key)
    private string? Validate(Student s)
    {
        s.NRP = (s.NRP ?? "").Trim();
        s.Nama = (s.Nama ?? "").Trim();
        s.Email = (s.Email ?? "").Trim();

        var kosong = new List<string>();
        if (s.NRP == "") kosong.Add("NRP");
        if (s.Nama == "") kosong.Add("Nama");
        if (string.IsNullOrWhiteSpace(s.Jurusan)) kosong.Add("Jurusan");
        if (string.IsNullOrWhiteSpace(s.Gender)) kosong.Add("Gender");
        if (s.Email == "") kosong.Add("Email");

        if (kosong.Count > 0)
            return "⚠ Data belum lengkap. Isi terlebih dahulu: " + string.Join(", ", kosong) + ".";

        if (_repository.NrpExists(s.NRP, s.id))
            return $"⚠ NRP {s.NRP} sudah terdaftar. NRP tidak boleh sama.";

        return null;
    }

    private void DeleteByid(int id)
    {
        _repository.Delete(id);
        LoadData();
        Reset();
        SuccessMessage = "✔ Data mahasiswa berhasil dihapus.";
    }

    private void Search()
    {
        var result = string.IsNullOrWhiteSpace(SearchText)
            ? _repository.GetAll()
            : _repository.Search(SearchText.Trim());

        Students.Clear();

        foreach (var student in result)
            Students.Add(student);

        RefreshStatistics();
    }

    private void ResetSearch()
    {
        SearchText = "";
        Search();
    }

    private void Reset()
    {
        SelectedStudent = null;
        FormStudent = new Student();
        ErrorMessage = "";
        SuccessMessage = "";
    }

    private void LoadToForm(Student s)
    {
        FormStudent = new Student
        {
            id = s.id,
            NRP = s.NRP,
            Nama = s.Nama,
            Jurusan = s.Jurusan,
            Gender = s.Gender,
            Email = s.Email
        };
        ErrorMessage = "";
        SuccessMessage = "";
    }

    private void RefreshStatistics()
    {
        OnPropertyChanged(nameof(TotalStudents));
        OnPropertyChanged(nameof(TotalTeknikInformatika));
        OnPropertyChanged(nameof(TotalTeknikElektro));
        OnPropertyChanged(nameof(TotalTeknikBiomedik));
        OnPropertyChanged(nameof(TotalTeknikKomputer));
        OnPropertyChanged(nameof(TotalTeknologiInformasi));
        OnPropertyChanged(nameof(TotalSistemInformasi));
        OnPropertyChanged(nameof(TotalLakiLaki));
        OnPropertyChanged(nameof(TotalPerempuan));
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propertyName));
    }
}