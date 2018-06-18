using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GrandTheftMultiplayer.Launcher.Helpers;

namespace GrandTheftMultiplayer.Launcher.Extensions
{
    public static class ListExtensions
    {
        public static async Task UpdateToTargetAsync<T>(this IList<T> targetCollection, ICollection<T> filteredCollection, TaskQueue taskQueue)
        {
            var cancellationTokenSource = new CancellationTokenSource();

            var itemsToRemove = new List<T>();
            try
            {
                await taskQueue.Enqueue(() => UpdateToFilteredTask(targetCollection, filteredCollection, itemsToRemove, cancellationTokenSource.Token), cancellationTokenSource);
            }
            catch (TaskCanceledException)
            {
                return;
            }

            // Add missing correct items to target collection.
            var itemsToAdd = filteredCollection.ToList();

            itemsToRemove.ForEach(i => targetCollection.Remove(i));
            itemsToAdd.ForEach(targetCollection.Add);
        }

        private static async Task UpdateToFilteredTask<T>(IList<T> targetCollection, ICollection<T> filteredCollection, ICollection<T> itemsToRemove, CancellationToken token)
        {
            await Task.Factory.StartNew(() =>
            {
                // Walk through target collection back to front.
                for (var i = targetCollection.Count - 1; i >= 0; i--)
                {
                    // New search initiated.
                    if (token.IsCancellationRequested) return;

                    var targetItem = targetCollection[i];

                    // Item in target collection is also in correct list. Leave it, remove from correct collection.
                    if (filteredCollection.Contains(targetItem))
                    {
                        filteredCollection.Remove(targetItem);
                        continue;
                    }

                    // Item not in correct collection, remove from target collection.
                    itemsToRemove.Add(targetItem);
                }
            }, token);
        }
    }
}
