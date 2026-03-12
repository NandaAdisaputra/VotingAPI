using Microsoft.EntityFrameworkCore;      // Library Entity Framework Core untuk koneksi database
using OsisVotingAPI.Data;                 // Mengakses AppDbContext (kelas penghubung aplikasi dengan database)

var builder = WebApplication.CreateBuilder(args);

// =====================================================
// KONFIGURASI DATABASE
// =====================================================

// Menambahkan AppDbContext ke dalam Dependency Injection Container
// Ini memungkinkan controller menggunakan database melalui constructor injection
builder.Services.AddDbContext<AppDbContext>(options =>
    // Menggunakan SQL Server sebagai database
    // Connection string diambil dari file appsettings.json
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


// =====================================================
// MENAMBAHKAN CONTROLLER API
// =====================================================

// Mengaktifkan sistem Controller untuk Web API
// Tanpa ini endpoint seperti /api/voting tidak akan berjalan
builder.Services.AddControllers();


// =====================================================
// SWAGGER (API DOCUMENTATION)
// =====================================================

// Digunakan untuk membuat dokumentasi API otomatis
builder.Services.AddEndpointsApiExplorer();

// Mengaktifkan Swagger generator
// Swagger akan membuat halaman untuk mencoba API langsung di browser
builder.Services.AddSwaggerGen();


// =====================================================
// MEMBANGUN APLIKASI
// =====================================================

var app = builder.Build();


// =====================================================
// SWAGGER HANYA UNTUK DEVELOPMENT
// =====================================================

// Jika environment adalah Development
// maka Swagger UI akan aktif
if (app.Environment.IsDevelopment())
{
    // Mengaktifkan middleware Swagger
    app.UseSwagger();

    // Menampilkan UI Swagger di browser
    // biasanya diakses di: https://localhost:xxxx/swagger
    app.UseSwaggerUI();
}


// =====================================================
// MIDDLEWARE AUTHORIZATION
// =====================================================

// Middleware untuk mengatur authorization
// Saat ini belum digunakan karena belum ada login token
app.UseAuthorization();


// =====================================================
// ROUTING CONTROLLER API
// =====================================================

// Menghubungkan endpoint API dengan controller
// contoh:
// api/voting/login
// api/voting/candidates
app.MapControllers();


// =====================================================
// MENJALANKAN APLIKASI
// =====================================================

// Menjalankan Web API server
app.Run();