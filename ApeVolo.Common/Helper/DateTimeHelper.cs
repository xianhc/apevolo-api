namespace ApeVolo.Common.Helper
{
    public static class DateTimeHelper
    {
        /// <summary>
        /// 格式化时间
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string FormatLongToTime(long time)
        {
            int day = 0;
            int hour = 0;
            int minute = 0;
            int second = 0;
            second = (int) (time / 1000);

            if (second > 60)
            {
                minute = second / 60;
                second %= 60;
            }

            if (minute > 60)
            {
                hour = minute / 60;
                minute %= 60;
            }

            if (hour <= 24) return hour + "小时" + minute + "分钟" + second + "秒";
            day = hour / 24;
            hour %= 24;

            return day + "天" + hour + "小时" + minute + "分钟" + second + "秒";
        }
    }
}