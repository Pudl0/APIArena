using APIArena.Models;
using APIArena.Server;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace APIArena.Services
{
    public class SettingService(DataContext _context)
    {
        public void SetSetting<T>(string key, T value)
        {
            var serializedValue = JsonConvert.SerializeObject(value);
            var type = typeof(T).AssemblyQualifiedName;

            var setting = _context.Settings.SingleOrDefault(s => s.Key == key);
            if (setting == null)
            {
                setting = new Setting
                {
                    Id = Guid.NewGuid(),
                    Key = key,
                    Value = serializedValue,
                    Type = type
                };
                _context.Settings.Add(setting);
            }
            else
            {
                setting.Value = serializedValue;
                setting.Type = type;
                _context.Settings.Update(setting);
            }

            _context.SaveChanges();
        }

        public T? GetSetting<T>(string key)
        {
            var setting = _context.Settings.SingleOrDefault(s => s.Key == key);
            if (setting != null)
            {
                return JsonConvert.DeserializeObject<T>(setting.Value);
            }

            return default;
        }

        public object? GetSetting(string key)
        {
            var setting = _context.Settings.SingleOrDefault(s => s.Key == key);
            if (setting != null && setting.Type != null)
            {
                var type = Type.GetType(setting.Type);
                return JsonConvert.DeserializeObject(setting.Value, type);
            }

            return null;
        }
    }
}
