using System;
using System.Diagnostics;
using System.IO;
using NHapi.Base.Model;
using NHapi.Base.Parser;
using NHapi.Model.V23.Message;
using System.Globalization;


// tutorial https://saravanansubramanian.com/hl72xnhapicreatemessage/
namespace hl7v2_dotnet_app
{
  class Program
  {
    static void Main(string[] args)
    {
      Console.WriteLine("Hello World!");

      try
      {
        // create the HL7 message
        // this AdtMessageFactory class is not from NHAPI but my own wrapper
        Console.WriteLine("Creating ADT A01 message...");

        var adtMessage = new ADT_A01();
        createMshSegment(adtMessage);
        createEvnSegment(adtMessage);
        createPidSegment(adtMessage);



        // print out the message that we constructed
        Console.WriteLine("Message was constructed successfully...\n");

        // create these parsers for the file encoding operations
        var pipeParser = new PipeParser();
        var xmlParser = new DefaultXMLParser();

        // serialize the message to pipe delimited output file
        Console.WriteLine("With pipeParser");
        WriteMessageFile(pipeParser, adtMessage, "./output", "message.hv2");
        // Console.WriteLine(pipeParser.Encode(adtMessage));

        Console.WriteLine("");

        // serialize the message to XML format output file
        Console.WriteLine("With xmlParser");
        WriteMessageFile(xmlParser, adtMessage, "./output", "message.xml");
        // Console.WriteLine(xmlParser.Encode(adtMessage));

      }
      catch (Exception e)
      {
        Console.WriteLine($"Error occured while creating HL7 message {e.Message}");
      }


    }

    private static void createEvnSegment(ADT_A01 adtMessage)
    {
      var evn = adtMessage.EVN;
      evn.EventTypeCode.Value = "A01";
      evn.RecordedDateTime.TimeOfAnEvent.Value = GetCurrentTimeStamp();
    }


    private static void createPidSegment(ADT_A01 adtMessage)
    {
      var pid = adtMessage.PID;
      var patientName = pid.GetPatientName(0);
      patientName.FamilyName.Value = "Muster";
      patientName.GivenName.Value = "Max";
      pid.SetIDPatientID.Value = "378785433211";
      var patientAddress = pid.GetPatientAddress(0);
      patientAddress.StreetAddress.Value = "Mustergasse 11";
      patientAddress.City.Value = "Musterhausen";
      patientAddress.StateOrProvince.Value = "SG";
      patientAddress.Country.Value = "CH";
    }

    private static void createMshSegment(ADT_A01 adtMessage)
    {
      var mshSegment = adtMessage.MSH;
      mshSegment.FieldSeparator.Value = "|";
      mshSegment.EncodingCharacters.Value = "^~\\&";
      mshSegment.SendingApplication.NamespaceID.Value = "Our System";
      mshSegment.SendingFacility.NamespaceID.Value = "Our Facility";
      mshSegment.ReceivingApplication.NamespaceID.Value = "Their Remote System";
      mshSegment.ReceivingFacility.NamespaceID.Value = "Their Remote Facility";
      mshSegment.DateTimeOfMessage.TimeOfAnEvent.Value = GetCurrentTimeStamp();
      mshSegment.MessageControlID.Value = GetSequenceNumber();
      mshSegment.MessageType.MessageType.Value = "ADT";
      mshSegment.MessageType.TriggerEvent.Value = "A01";
      mshSegment.VersionID.Value = "2.3";
      mshSegment.ProcessingID.ProcessingID.Value = "P";
    }

    private static string GetCurrentTimeStamp()
    {
      return DateTime.Now.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture);
    }

    private static string GetSequenceNumber()
    {
      const string facilityNumberPrefix = "1234"; // some arbitrary prefix for the facility
      return facilityNumberPrefix + GetCurrentTimeStamp();
    }


    private static void WriteMessageFile(ParserBase parser, IMessage hl7Message, string outputDirectory, string outputFileName)
    {
      if (!Directory.Exists(outputDirectory))
        Directory.CreateDirectory(outputDirectory);

      var fileName = Path.Combine(outputDirectory, outputFileName);


      if (File.Exists(fileName))
        File.Delete(fileName);
      File.WriteAllText(fileName, parser.Encode(hl7Message));
    }

  }
}
