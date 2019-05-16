﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeMasterDotNet;

namespace MeshSim
{
    public class MeasurementPost: IComparable, IEquatable<MeasurementPost>
    {
        public int Id { get; set; }
        int value;
        long timeStamp;
        public long MainTimeStamp { get; set; }
        public int AdCounter { get; set; }

        internal bool reported;

        public MeasurementPost(int id, int value, long timeStamp)
        {
            this.timeStamp = timeStamp;
            this.Id = id;
            this.value = value;
            this.reported = false;
            this.AdCounter = 0;
        }

        override public string ToString()
        {
            return string.Format("M{0:X4}, {1}, {2}, {3}", Id, timeStamp, AdCounter, value);
        }

        public int CompareTo(object obj)
        {
            if (obj is MeasurementPost post)
            {
                return timeStamp.CompareTo(post.timeStamp);
            }

            return -1;
        }

        public bool Equals(MeasurementPost other)
        {
            return (timeStamp == other.timeStamp) && (Id == other.Id);
        }
    }
}
