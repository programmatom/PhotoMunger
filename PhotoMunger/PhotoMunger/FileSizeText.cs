/*
 *  Copyright © 2010-2016 Thomas R. Lawrence
 * 
 *  GNU General Public License
 * 
 *  This file is part of PhotoMunger
 * 
 *  PhotoMunger is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with this program. If not, see <http://www.gnu.org/licenses/>.
 * 
*/
using System;
using System.IO;

namespace AdaptiveImageSizeReducer
{
    public static class FileSizeText
    {
        public static long GetFileLength(string path)
        {
            using (Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                return stream.Length;
            }
        }

        private static readonly string[] FileSizeSuffixes = new string[] { "B", "KB", "MB", "GB", "TB" };
        public static string GetFileSizeString(long length)
        {
            double scaled = length;
            foreach (string suffix in FileSizeSuffixes)
            {
                if (Math.Round(scaled) < 1000)
                {
                    return scaled.ToString(scaled >= 1 ? "G3" : "G2") + suffix;
                }
                scaled /= 1024;
            }
            return (scaled * 1024).ToString("N0") + FileSizeSuffixes[FileSizeSuffixes.Length - 1];
        }

        public static string GetFileSizeString(string path)
        {
            return GetFileSizeString(GetFileLength(path));
        }
    }
}
