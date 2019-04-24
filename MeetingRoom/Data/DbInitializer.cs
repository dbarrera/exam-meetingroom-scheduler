namespace MeetingRoom.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ExamContext context)
        {
            context.SaveChanges();
        }
    }
}
