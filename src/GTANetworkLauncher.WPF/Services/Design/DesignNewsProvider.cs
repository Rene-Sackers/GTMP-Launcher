using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GrandTheftMultiplayer.Launcher.Models.Forum;
using GrandTheftMultiplayer.Launcher.Services.Interfaces;

namespace GrandTheftMultiplayer.Launcher.Services.Design
{
    public class DesignNewsProvider : INewsProvider
    {
        public Task<IEnumerable<ForumPostItem>> GetNewsAsync()
        {
            return Task.FromResult(CreateExampleNews());
        }

        private static IEnumerable<ForumPostItem> CreateExampleNews()
        {
            for (var i = 0; i < 5; i++)
            {
                yield return new ForumPostItem
                {
                    Title = "News item " + i,
                    Author = "Msk",
                    Text = "Things",
                    Url = "https://gt-mp.net/",
                    Id = i,
                    Timestamp = DateTime.Now.AddDays(-i)
                };
            }
        }
    }
}
