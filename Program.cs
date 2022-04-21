using System;
using System.Diagnostics;
using System.IO;
using NHapi.Base.Model;
using NHapi.Base.Parser;
using NHapi.Model.V23.Message;


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
        LogToDebugConsole("Creating ADT A01 message...");
       
        var adtMessage = new ADT_A01();
        var mshSegment = adtMessage.MSH;
                mshSegment.FieldSeparator.Value = "|";
                mshSegment.EncodingCharacters.Value = "^~\\&";
                mshSegment.SendingApplication.NamespaceID.Value = "Our System";
                mshSegment.SendingFacility.NamespaceID.Value = "Our Facility";
                mshSegment.ReceivingApplication.NamespaceID.Value = "Their Remote System";
                mshSegment.ReceivingFacility.NamespaceID.Value = "Their Remote Facility";
                // mshSegment.DateTimeOfMessage.TimeOfAnEvent.Value = currentDateTimeString;
                // mshSegment.MessageControlID.Value = GetSequenceNumber();
                mshSegment.MessageType.MessageType.Value = "ADT";
                mshSegment.MessageType.TriggerEvent.Value = "A01";
                mshSegment.VersionID.Value = "2.3";
                mshSegment.ProcessingID.ProcessingID.Value = "P";


        // create these parsers for the file encoding operations
        var pipeParser = new PipeParser();
        var xmlParser = new DefaultXMLParser();

        // print out the message that we constructed
        LogToDebugConsole("Message was constructed successfully..." + "\n");


        // serialize the message to pipe delimited output file
        //WriteMessageFile(pipeParser, adtMessage, "C:\\HL7TestOutputs", "testPipeDelimitedOutputFile.txt");
        LogToDebugConsole("With pipeParser");
        LogToDebugConsole(pipeParser.Encode(adtMessage));

        // serialize the message to XML format output file
        //WriteMessageFile(xmlParser, adtMessage, "C:\\HL7TestOutputs", "testXmlOutputFile.xml");
        LogToDebugConsole("With xmlParser");
        LogToDebugConsole(xmlParser.Encode(adtMessage));

      }
      catch (Exception e)
      {
        LogToDebugConsole($"Error occured while creating HL7 message {e.Message}");
      }


    }



    // private static void WriteMessageFile(ParserBase parser, IMessage hl7Message, string outputDirectory, string outputFileName)
    // {
    //   if (!Directory.Exists(outputDirectory))
    //     Directory.CreateDirectory(outputDirectory);

    //   var fileName = Path.Combine(outputDirectory, outputFileName);

    //   LogToDebugConsole("Writing data to file...");

    //   if (File.Exists(fileName))
    //     File.Delete(fileName);
    //   File.WriteAllText(fileName, parser.Encode(hl7Message));
    //   LogToDebugConsole($"Wrote data to file {fileName} successfully...");
    // }

    private static void LogToDebugConsole(string informationToLog)
    {
    //   Debug.WriteLine(informationToLog);
      Console.WriteLine(informationToLog);
    }
  }
}
