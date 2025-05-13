using Domain.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Services.External_Services;

namespace Domain.Manager
{
    public class PatientManager : IPatientManager
    {
        private readonly ILogger<PatientManager> _logger;
        private readonly string _filePath;
        private readonly List<Patient> _patients;
        private readonly IPatientCodeService _patientCodeService;

        private static readonly string[] _bloodGroups = {
            "A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-"
        };

        public PatientManager(string filePath, ILogger<PatientManager> logger, IPatientCodeService patientCodeService)
        {
            _logger = logger;
            _filePath = filePath;
            _patientCodeService = patientCodeService;

            _patients = File.Exists(_filePath)
                ? File.ReadAllLines(_filePath)
                      .Select(line => line.Split(','))
                      .Select(p => new Patient
                      {
                          Name = p[0],
                          LastName = p[1],
                          CI = p[2],
                          BloodGroup = p[3],
                          PatientCode = p.Length >= 5 ? p[4] : ""
                      })
                      .ToList()
                : new List<Patient>();
        }

        public IEnumerable<Patient> GetAll() => _patients;

        public Patient GetByCi(string ci) =>
            _patients.FirstOrDefault(p => p.CI == ci);

        public async Task CreateAsync(Patient patient)
        {
            var rnd = new Random();
            patient.BloodGroup = _bloodGroups[rnd.Next(_bloodGroups.Length)];
            _logger.LogInformation("Paciente creado con grupo sanguíneo: {Grupo}", patient.BloodGroup);

            patient.PatientCode = await _patientCodeService.GetPatientCodeAsync(patient.Name, patient.LastName, patient.CI);
            _logger.LogInformation("Patient Code asignado: {PatientCode}", patient.PatientCode);

            _patients.Add(patient);
            Persist();
            _logger.LogInformation("Paciente {Nombre} guardado correctamente.", patient.Name);
        }

        public bool Update(string ci, string name, string lastName)
        {
            var existing = GetByCi(ci);
            if (existing == null)
            {
                _logger.LogWarning("Paciente con CI {Ci} no encontrado.", ci);
                return false;
            }

            _logger.LogInformation("Actualizando paciente con CI {Ci}.", ci);
            existing.Name = name;
            existing.LastName = lastName;
            Persist();

            _logger.LogInformation("Paciente con CI {Ci} actualizado.", ci);
            return true;
        }

        public bool Delete(string ci)
        {
            var existing = GetByCi(ci);
            if (existing == null)
            {
                _logger.LogWarning("Paciente con CI {Ci} no encontrado.", ci);
                return false;
            }

            _patients.Remove(existing);
            Persist();
            _logger.LogInformation("Paciente con CI {Ci} eliminado.", ci);
            return true;
        }

        private void Persist() =>
            File.WriteAllLines(_filePath,
                _patients.Select(p =>
                    $"{p.Name},{p.LastName},{p.CI},{p.BloodGroup},{p.PatientCode}"
                )
            );
    }
}
