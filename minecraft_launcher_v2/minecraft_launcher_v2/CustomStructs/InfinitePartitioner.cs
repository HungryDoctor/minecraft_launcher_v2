using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace minecraft_launcher_v2.CustomStructs
{
    public class InfinitePartitioner : Partitioner<bool>
    {
        private static IEnumerator<bool> InfiniteEnumerator()
        {
            while (true) yield return true;
        }

        

        public override bool SupportsDynamicPartitions
        {
            get
            {
                return true;
            }
        }

        public override IList<IEnumerator<bool>> GetPartitions(int partitionCount)
        {
            if (partitionCount < 1)
            {
                throw new ArgumentOutOfRangeException("partitionCount");
            }

            return Enumerable.Range(0, partitionCount).Select(x => InfiniteEnumerator()).ToArray();
        }

        public override IEnumerable<bool> GetDynamicPartitions()
        {
            return new InfiniteEnumerators();
        }



        private class InfiniteEnumerators : IEnumerable<bool>
        {
            public IEnumerator<bool> GetEnumerator()
            {
                return InfiniteEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }
        }

    }

}
